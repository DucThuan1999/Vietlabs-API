using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VietLab.Data;
using VietLab.Models;
using VietLab.Models.DTOs;
using VietLab.Services;

namespace VietLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;
    private readonly ModulePermissionService _modulePerm;
    private readonly AccessTokenService _tokenService;

    public AuthController(
        ApplicationDbContext context,
        ILogger<AuthController> logger,
        ModulePermissionService modulePerm,
        AccessTokenService tokenService)
    {
        _context = context;
        _logger = logger;
        _modulePerm = modulePerm;
        _tokenService = tokenService;
    }

    private async Task<UserInfo> MapToUserInfoAsync(Account account, CancellationToken ct = default)
    {
        var codes = await _modulePerm.GetGrantedCodesAsync(account.AccountId, ct);
        return new UserInfo
        {
            AccountId = account.AccountId,
            EmployeeId = account.EmployeeId,
            UserName = account.UserName,
            FullName = account.Employee?.FullName ?? "",
            Email = account.Employee?.Email ?? "",
            Department = account.Employee?.Department?.NameVi ?? "",
            Role = account.Employee?.Role ?? "",
            Title = account.Employee?.Title ?? "",
            Status = account.Status,
            GrantedPermissionCodes = codes
        };
    }

    /// <summary>
    /// Đăng nhập hệ thống
    /// </summary>
    /// <param name="request">Thông tin đăng nhập (UserName và Password)</param>
    /// <returns>Thông tin user và token nếu đăng nhập thành công</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new LoginResponse
            {
                Success = false,
                Message = "Tên đăng nhập và mật khẩu không được để trống"
            });
        }

        try
        {
            // Tìm account theo username
            var account = await _context.Accounts
                .Include(a => a.Employee).ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(a => a.UserName == request.UserName && a.Status == "Active");

            if (account == null)
            {
                _logger.LogWarning("Login failed: User not found - {UserName}", request.UserName);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Không tìm thấy tài khoản hoặc tài khoản chưa được kích hoạt. Vui lòng kiểm tra tên đăng nhập."
                });
            }

            // Kiểm tra password (so sánh hash)
            // Lưu ý: Trong production nên dùng BCrypt hoặc ASP.NET Identity
            var isValid = VerifyPassword(request.Password, account.PasswordHash);
            
            if (!isValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user - {UserName}", request.UserName);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Mật khẩu không đúng. Vui lòng thử lại hoặc dùng chức năng đổi mật khẩu nếu bạn quên mật khẩu."
                });
            }

            // Kiểm tra employee có tồn tại không
            if (account.Employee == null)
            {
                _logger.LogWarning("Login failed: Employee not found for account - {AccountId}", account.AccountId);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Tài khoản không liên kết với nhân viên"
                });
            }

            // Tạo access token và refresh token
            var accessToken = _tokenService.Sign(account.AccountId, account.UserName);
            var (refreshTokenEntity, refreshTokenRaw) = await GenerateAndSaveRefreshToken(account.AccountId);

            // Tạo response với thông tin user
            var response = new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                    User = await MapToUserInfoAsync(account),
                Token = accessToken,
                RefreshToken = refreshTokenRaw,
                TokenExpiresAt = DateTime.UtcNow.Add(_tokenService.AccessTokenLifetime),
                RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt
            };

            _logger.LogInformation("Login successful: {UserName}", request.UserName);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {UserName}", request.UserName);
            return StatusCode(500, new LoginResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi trong quá trình đăng nhập"
            });
        }
    }

    /// <summary>
    /// Hash password (simple implementation - nên dùng BCrypt trong production)
    /// </summary>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// Verify password với hash hiện có.
    /// </summary>
    private bool VerifyPassword(string password, string storedHash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == storedHash;
    }

    /// <summary>
    /// Generate và lưu refresh token vào database. Chỉ hash của token được lưu (giống cách lưu
    /// password) — nếu DB bị lộ/truy vấn trái phép, kẻ tấn công không lấy được refresh token dùng ngay
    /// được mà không cần crack. Trả về (entity, rawToken) — chỉ rawToken được gửi cho client.
    /// </summary>
    private async Task<(Models.RefreshToken Entity, string RawToken)> GenerateAndSaveRefreshToken(Guid accountId)
    {
        // Revoke tất cả refresh token cũ của account này
        var oldTokens = await _context.RefreshTokens
            .Where(rt => rt.AccountId == accountId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var oldToken in oldTokens)
        {
            oldToken.IsRevoked = true;
            oldToken.RevokedReason = "Replaced by new token";
        }

        // Tạo refresh token mới
        var rawToken = GenerateRefreshTokenString();
        var refreshToken = new Models.RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            AccountId = accountId,
            Token = HashRefreshToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token hết hạn sau 7 ngày
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return (refreshToken, rawToken);
    }

    /// <summary>
    /// Generate refresh token string (random secure token)
    /// </summary>
    private string GenerateRefreshTokenString()
    {
        var randomBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    /// <summary>Hash refresh token để lưu trữ (token đã có đủ entropy ngẫu nhiên nên không cần salt).</summary>
    private static string HashRefreshToken(string rawToken)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Refresh access token bằng refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new RefreshTokenResponse
            {
                Success = false,
                Message = "Refresh token không được để trống"
            });
        }

        try
        {
            // Tìm refresh token trong database (so khớp theo hash — DB chỉ lưu hash, không lưu raw token)
            var requestTokenHash = HashRefreshToken(request.RefreshToken);
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.Account!)
                    .ThenInclude(a => a.Employee)
                .Include(rt => rt.Account!)
                .FirstOrDefaultAsync(rt => rt.Token == requestTokenHash && !rt.IsRevoked);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found or revoked: {Token}", request.RefreshToken.Substring(0, Math.Min(20, request.RefreshToken.Length)));
                return Unauthorized(new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token không hợp lệ hoặc đã bị thu hồi"
                });
            }

            // Kiểm tra refresh token đã hết hạn chưa
            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired: {TokenId}", refreshToken.RefreshTokenId);
                refreshToken.IsRevoked = true;
                refreshToken.RevokedReason = "Expired";
                await _context.SaveChangesAsync();

                return Unauthorized(new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token đã hết hạn"
                });
            }

            // Kiểm tra account có còn active không
            if (refreshToken.Account == null || refreshToken.Account.Status != "Active")
            {
                _logger.LogWarning("Account not found or not active for refresh token: {AccountId}", refreshToken.AccountId);
                return Unauthorized(new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Tài khoản không còn hoạt động"
                });
            }

            // Revoke refresh token cũ (rotate token)
            refreshToken.IsRevoked = true;
            refreshToken.RevokedReason = "Rotated";

            // Tạo refresh token mới
            var (newRefreshTokenEntity, newRefreshTokenRaw) = await GenerateAndSaveRefreshToken(refreshToken.AccountId);

            // Tạo access token mới
            var newAccessToken = _tokenService.Sign(refreshToken.Account.AccountId, refreshToken.Account.UserName);

            var accFull = await _context.Accounts
                .Include(a => a.Employee).ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(a => a.AccountId == refreshToken.AccountId);

            _logger.LogInformation("Token refreshed successfully for account: {AccountId}", refreshToken.AccountId);

            return Ok(new RefreshTokenResponse
            {
                Success = true,
                Message = "Token đã được làm mới thành công",
                Token = newAccessToken,
                RefreshToken = newRefreshTokenRaw,
                TokenExpiresAt = DateTime.UtcNow.Add(_tokenService.AccessTokenLifetime),
                RefreshTokenExpiresAt = newRefreshTokenEntity.ExpiresAt,
                User = accFull != null ? await MapToUserInfoAsync(accFull) : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new RefreshTokenResponse
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi làm mới token"
            });
        }
    }

    /// <summary>
    /// Đổi mật khẩu (yêu cầu đăng nhập)
    /// </summary>
    /// <param name="request">CurrentPassword và NewPassword</param>
    /// <returns>Thành công hoặc lỗi</returns>
    [HttpPost("change-password")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(new { success = false, message = "Mật khẩu hiện tại và mật khẩu mới không được để trống." });
        }

        // Validate độ dài mật khẩu mới (tùy chọn)
        if (request.NewPassword.Length < 6)
        {
            return BadRequest(new { success = false, message = "Mật khẩu mới phải có ít nhất 6 ký tự." });
        }

        var accountIdStr = User.FindFirst("AccountId")?.Value;
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(accountIdStr) || !Guid.TryParse(accountIdStr, out var accountId))
        {
            _logger.LogWarning("ChangePassword: Invalid or missing AccountId in token");
            return Unauthorized(new { success = false, message = "Token không hợp lệ." });
        }

        try
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.Status == "Active");

            if (account == null)
            {
                _logger.LogWarning("ChangePassword: Account not found or inactive - {AccountId}", accountId);
                return Unauthorized(new { success = false, message = "Tài khoản không tồn tại hoặc đã bị vô hiệu hóa." });
            }

            if (!VerifyPassword(request.CurrentPassword, account.PasswordHash))
            {
                _logger.LogWarning("ChangePassword: Current password incorrect for user - {UserName}", account.UserName);
                return BadRequest(new { success = false, message = "Mật khẩu hiện tại không đúng." });
            }

            account.PasswordHash = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for user: {UserName}", account.UserName);
            return Ok(new { success = true, message = "Đổi mật khẩu thành công." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for account: {AccountId}", accountId);
            return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi đổi mật khẩu." });
        }
    }

    /// <summary>
    /// Lấy lại profile + ma trận quyền (body: accessToken, refreshToken).
    /// </summary>
    [HttpPost("me")]
    [AllowAnonymous]
    public async Task<IActionResult> Me([FromBody] MeRequest? body)
    {
        var token = body?.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { success = false, message = "Thiếu accessToken." });

        if (!_tokenService.TryValidate(token, out var accountId, out var userName))
            return Unauthorized(new { success = false, message = "Token không hợp lệ hoặc đã hết hạn." });

        var account = await _context.Accounts
            .Include(a => a.Employee).ThenInclude(e => e!.Department)
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UserName == userName && a.Status == "Active");

        if (account == null)
            return Unauthorized(new { success = false, message = "Tài khoản không hợp lệ." });

        var user = await MapToUserInfoAsync(account);
        return Ok(new
        {
            success = true,
            user,
            account = new { accountId = account.AccountId, userName = account.UserName },
            token = new { accessToken = token, refreshToken = body?.RefreshToken },
            refreshToken = body?.RefreshToken
        });
    }

    /// <summary>
    /// Test endpoint để kiểm tra Bearer token authentication
    /// </summary>
    /// <returns>Thông tin user từ token nếu authenticated thành công</returns>
    [HttpGet("test-auth")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult TestAuth()
    {
        try
        {
            // Lấy thông tin từ claims
            var accountId = User.FindFirst("AccountId")?.Value;
            var userName = User.Identity?.Name;
            var employeeId = User.FindFirst("EmployeeId")?.Value;
            var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            var response = new
            {
                success = true,
                message = "Authentication thành công! Token hợp lệ.",
                user = new
                {
                    accountId = accountId,
                    userName = userName,
                    employeeId = employeeId,
                    fullName = fullName,
                    email = email,
                    isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    authenticationType = User.Identity?.AuthenticationType
                },
                claims = User.Claims.Select(c => new
                {
                    type = c.Type,
                    value = c.Value
                }).ToList()
            };

            _logger.LogInformation("Test auth successful for user: {UserName}", userName);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test auth endpoint");
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi khi kiểm tra authentication",
                error = ex.Message
            });
        }
    }
}

