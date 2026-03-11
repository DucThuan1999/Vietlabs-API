namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: Package - AnalysisItem (many-to-many)
/// Một gói có thể chứa nhiều chỉ tiêu, một chỉ tiêu có thể thuộc nhiều gói
/// </summary>
public class PackageAnalysisItem
{
    public Guid PackageAnalysisItemId { get; set; }
    public Guid PackageId { get; set; } // Foreign key đến Package
    public Guid AnalysisItemId { get; set; } // Foreign key đến AnalysisItem

    // Thông tin bổ sung
    public int? DisplayOrder { get; set; } // Thứ tự hiển thị trong gói
    public bool IsRequired { get; set; } = true; // Bắt buộc hay không (có thể bỏ trong gói tùy chỉnh)
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation Properties
    public Package? Package { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
