namespace VietLab.Models.DTOs;

public class ChangePasswordRequest
{
    /// <summary>Mật khẩu hiện tại</summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>Mật khẩu mới</summary>
    public string NewPassword { get; set; } = string.Empty;
}
