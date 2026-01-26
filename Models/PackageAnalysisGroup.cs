namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: Package - AnalysisGroup (many-to-many)
/// Một gói có thể chứa nhiều nhóm chỉ tiêu, một nhóm chỉ tiêu có thể thuộc nhiều gói
/// </summary>
public class PackageAnalysisGroup
{
    public Guid PackageAnalysisGroupId { get; set; }
    public Guid PackageId { get; set; } // Foreign key đến Package
    public Guid AnalysisGroupId { get; set; } // Foreign key đến AnalysisGroup

    // Thông tin bổ sung
    public int? DisplayOrder { get; set; } // Thứ tự hiển thị trong gói
    public bool IsRequired { get; set; } = true; // Bắt buộc hay không (có thể bỏ trong gói tùy chỉnh)
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Package? Package { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
}

