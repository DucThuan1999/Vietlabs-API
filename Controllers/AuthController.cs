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

    public AuthController(ApplicationDbContext context, ILogger<AuthController> logger, ModulePermissionService modulePerm)
    {
        _context = context;
        _logger = logger;
        _modulePerm = modulePerm;
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
            // Bypass test với password admin
            if (request.Password == "admin")
            {
                // Tìm account đầu tiên có status Active, hoặc tìm theo username nếu có
                var adminAccount = await _context.Accounts
                    .Include(a => a.Employee).ThenInclude(e => e!.Department)
                    .FirstOrDefaultAsync(a => 
                        (string.IsNullOrEmpty(request.UserName) || a.UserName == request.UserName) 
                        && a.Status == "Active");

                if (adminAccount == null)
                {
                    // Nếu không tìm thấy account, tìm account đầu tiên có status Active
                    adminAccount = await _context.Accounts
                        .Include(a => a.Employee).ThenInclude(e => e!.Department)
                        .FirstOrDefaultAsync(a => a.Status == "Active");
                    
                    if (adminAccount == null)
                    {
                        _logger.LogWarning("Admin bypass: No active account found in database");
                        return Unauthorized(new LoginResponse
                        {
                            Success = false,
                            Message = "Không tìm thấy account nào trong hệ thống"
                        });
                    }
                }

                // Tạo access token và refresh token
                var adminAccessToken = GenerateSimpleToken(adminAccount.AccountId, adminAccount.UserName);
                var adminRefreshToken = await GenerateAndSaveRefreshToken(adminAccount.AccountId);

                // Tạo response với account tìm được
                var adminUser = await MapToUserInfoAsync(adminAccount);
                if (adminAccount.Employee == null)
                {
                    if (string.IsNullOrWhiteSpace(adminUser.FullName)) adminUser.FullName = "Admin User";
                    if (string.IsNullOrWhiteSpace(adminUser.Email)) adminUser.Email = "admin@viet-labs.com";
                    if (string.IsNullOrWhiteSpace(adminUser.Department)) adminUser.Department = "IT";
                    if (string.IsNullOrWhiteSpace(adminUser.Role)) adminUser.Role = "Admin";
                    if (string.IsNullOrWhiteSpace(adminUser.Title)) adminUser.Title = "Administrator";
                }

                var adminResponse = new LoginResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công (Admin Bypass)",
                    User = adminUser,
                    Token = adminAccessToken,
                    RefreshToken = adminRefreshToken.Token,
                    TokenExpiresAt = DateTime.UtcNow.AddHours(1), // Access token hết hạn sau 1 giờ
                    RefreshTokenExpiresAt = adminRefreshToken.ExpiresAt
                };

                _logger.LogInformation("Admin bypass login successful: {UserName}", request.UserName);
                return Ok(adminResponse);
            }

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
            var passwordHash = HashPassword(request.Password);
            var isValid = VerifyPassword(request.Password, account.PasswordHash, request.UserName);
            
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
            var accessToken = GenerateSimpleToken(account.AccountId, account.UserName);
            var refreshToken = await GenerateAndSaveRefreshToken(account.AccountId);

            // Tạo response với thông tin user
            var response = new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                    User = await MapToUserInfoAsync(account),
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                TokenExpiresAt = DateTime.UtcNow.AddHours(1), // Access token hết hạn sau 1 giờ
                RefreshTokenExpiresAt = refreshToken.ExpiresAt
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
    /// Verify password với hash hiện có
    /// </summary>
    private bool VerifyPassword(string password, string storedHash, string userName)
    {
        // Nếu storedHash là format cũ từ seed data (hashed-password-X)
        if (storedHash.StartsWith("hashed-password-"))
        {
            // Đây là seed data, cho phép các password đơn giản để test
            var simplePasswords = new[] { "password", "123456", "admin", userName.ToLower() };
            
            // Cho phép password đơn giản
            if (simplePasswords.Contains(password.ToLower()))
            {
                return true;
            }
            
            // Nếu password nhập vào chính là storedHash (tương thích với seed data)
            // Ví dụ: password = "hashed-password-7" và storedHash = "hashed-password-7"
            if (password == storedHash)
            {
                return true;
            }
            
            return false;
        }

        // Nếu password nhập vào chính là storedHash (fallback cho trường hợp đặc biệt)
        if (password == storedHash)
        {
            return true;
        }

        // So sánh hash thông thường
        var passwordHash = HashPassword(password);
        return passwordHash == storedHash;
    }

    /// <summary>
    /// Generate simple token (có thể thay bằng JWT token)
    /// </summary>
    private string GenerateSimpleToken(Guid accountId, string userName)
    {
        // Simple token: Base64 của AccountId + UserName + Timestamp
        // Trong production nên dùng JWT
        var tokenData = $"{accountId}:{userName}:{DateTime.UtcNow:yyyyMMddHHmmss}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData));
    }

    /// <summary>
    /// Generate và lưu refresh token vào database
    /// </summary>
    private async Task<Models.RefreshToken> GenerateAndSaveRefreshToken(Guid accountId)
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
        var refreshToken = new Models.RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            AccountId = accountId,
            Token = GenerateRefreshTokenString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token hết hạn sau 7 ngày
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
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
            // Tìm refresh token trong database
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.Account!)
                    .ThenInclude(a => a.Employee)
                .Include(rt => rt.Account!)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

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
            var newRefreshToken = await GenerateAndSaveRefreshToken(refreshToken.AccountId);

            // Tạo access token mới
            var newAccessToken = GenerateSimpleToken(refreshToken.Account.AccountId, refreshToken.Account.UserName);

            var accFull = await _context.Accounts
                .Include(a => a.Employee).ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(a => a.AccountId == refreshToken.AccountId);

            _logger.LogInformation("Token refreshed successfully for account: {AccountId}", refreshToken.AccountId);

            return Ok(new RefreshTokenResponse
            {
                Success = true,
                Message = "Token đã được làm mới thành công",
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                TokenExpiresAt = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
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

            if (!VerifyPassword(request.CurrentPassword, account.PasswordHash, account.UserName))
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

        Guid accountId;
        string userName;
        try
        {
            var tokenString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var tokenParts = tokenString.Split(':');
            if (tokenParts.Length < 2 || !Guid.TryParse(tokenParts[0], out accountId))
                return Unauthorized(new { success = false, message = "Token không hợp lệ." });
            userName = tokenParts[1];
        }
        catch
        {
            return Unauthorized(new { success = false, message = "Token không hợp lệ." });
        }

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

