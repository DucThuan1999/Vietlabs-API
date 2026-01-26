namespace VietLab.Models;

/// <summary>
/// Gói phân tích - chứa nhiều nhóm chỉ tiêu
/// </summary>
public class Package
{
    public Guid PackageId { get; set; }
    public string? PackageCode { get; set; } // Mã gói
    public string? NameVi { get; set; } // Tên gói tiếng Việt
    public string? NameEn { get; set; } // Tên gói tiếng Anh
    public string? Description { get; set; } // Mô tả gói
    public decimal? DefaultPrice { get; set; } // Giá mặc định của gói

    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation: 1 Package có nhiều PackageAnalysisGroup (many-to-many với AnalysisGroup)
    public ICollection<PackageAnalysisGroup> PackageAnalysisGroups { get; set; } = new List<PackageAnalysisGroup>();
}

