namespace VietLab.Models;

/// <summary>
/// Cấu hình singleton tên hiển thị thay NĐ107 (giấy phép đăng ký) trên UI.
/// Field cố định trên form settings: "GIẤY PHÉP ĐĂNG KÝ"; giá trị lưu tại DisplayName.
/// </summary>
public class RegistrationPermitLabelConfig
{
    public Guid RegistrationPermitLabelConfigId { get; set; }

    /// <summary>
    /// Tên hiển thị trên UI (mặc định seed: "NĐ 22/2026").
    /// </summary>
    public string DisplayName { get; set; } = "NĐ 22/2026";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}
