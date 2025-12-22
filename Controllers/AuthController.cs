using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VietLab.Data;
using VietLab.Models;
using VietLab.Models.DTOs;

namespace VietLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
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
                    .Include(a => a.Employee)
                    .Include(a => a.Permission)
                    .FirstOrDefaultAsync(a => 
                        (string.IsNullOrEmpty(request.UserName) || a.UserName == request.UserName) 
                        && a.Status == "Active");

                if (adminAccount == null)
                {
                    // Nếu không tìm thấy account, tìm account đầu tiên có status Active
                    adminAccount = await _context.Accounts
                        .Include(a => a.Employee)
                        .Include(a => a.Permission)
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

                // Tạo response với account tìm được
                var adminResponse = new LoginResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công (Admin Bypass)",
                    User = new UserInfo
                    {
                        AccountId = adminAccount.AccountId,
                        EmployeeId = adminAccount.EmployeeId,
                        UserName = adminAccount.UserName,
                        FullName = adminAccount.Employee?.FullName ?? "Admin User",
                        Email = adminAccount.Employee?.Email ?? "admin@viet-labs.com",
                        Department = adminAccount.Employee?.Department ?? "IT",
                        Role = adminAccount.Employee?.Role ?? "Admin",
                        Title = adminAccount.Employee?.Title ?? "Administrator",
                        PermissionId = adminAccount.PermissionId,
                        PermissionName = adminAccount.Permission?.Name ?? "Full Access",
                        PermissionCode = adminAccount.Permission?.PermissionCode ?? "ADMIN",
                        Status = adminAccount.Status
                    },
                    Token = GenerateSimpleToken(adminAccount.AccountId, adminAccount.UserName)
                };

                _logger.LogInformation("Admin bypass login successful: {UserName}", request.UserName);
                return Ok(adminResponse);
            }

            // Tìm account theo username
            var account = await _context.Accounts
                .Include(a => a.Employee)
                .Include(a => a.Permission)
                .FirstOrDefaultAsync(a => a.UserName == request.UserName && a.Status == "Active");

            if (account == null)
            {
                _logger.LogWarning("Login failed: User not found - {UserName}", request.UserName);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
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
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
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

            // Tạo response với thông tin user
            var response = new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new UserInfo
                {
                    AccountId = account.AccountId,
                    EmployeeId = account.EmployeeId,
                    UserName = account.UserName,
                    FullName = account.Employee.FullName,
                    Email = account.Employee.Email,
                    Department = account.Employee.Department,
                    Role = account.Employee.Role,
                    Title = account.Employee.Title,
                    PermissionId = account.PermissionId,
                    PermissionName = account.Permission?.Name ?? "",
                    PermissionCode = account.Permission?.PermissionCode ?? "",
                    Status = account.Status
                },
                Token = GenerateSimpleToken(account.AccountId, account.UserName) // Simple token, có thể thay bằng JWT
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
            var permissionId = User.FindFirst("PermissionId")?.Value;
            var permissionCode = User.FindFirst("PermissionCode")?.Value;
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
                    permissionId = permissionId,
                    permissionCode = permissionCode,
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

