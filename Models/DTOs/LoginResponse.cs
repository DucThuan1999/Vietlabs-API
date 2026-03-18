namespace VietLab.Models.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserInfo? User { get; set; }
    public string? Token { get; set; } // Access token
    public string? RefreshToken { get; set; } // Refresh token để lấy access token mới
    public DateTime? TokenExpiresAt { get; set; } // Thời gian hết hạn của access token
    public DateTime? RefreshTokenExpiresAt { get; set; } // Thời gian hết hạn của refresh token
}

public class UserInfo
{
    public Guid AccountId { get; set; }
    public Guid EmployeeId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    /// <summary>Mã quyền dạng Module.Action (ví dụ Quotation.View, Admin.Edit).</summary>
    public List<string> GrantedPermissionCodes { get; set; } = new();
}

