namespace VietLab.Models;

/// <summary>
/// Gói phân tích - chứa nhiều chỉ tiêu (AnalysisItem)
/// </summary>
public class Package
{
    public Guid PackageId { get; set; }
    public string? PackageCode { get; set; } // Mã gói
    public string? NameVi { get; set; } // Tên gói tiếng Việt
    public string? NameEn { get; set; } // Tên gói tiếng Anh
    public string? Description { get; set; } // Mô tả gói
    public decimal? DefaultPrice { get; set; } // Giá mặc định của gói
    public string? PublishedGroupCode { get; set; } // Phương pháp
    public Guid? SampleMatrixId { get; set; } // Foreign key đến SampleMatrix (Nền mẫu)

    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation: 1 Package có nhiều PackageAnalysisItem (many-to-many với AnalysisItem)
    public ICollection<PackageAnalysisItem> PackageAnalysisItems { get; set; } = new List<PackageAnalysisItem>();

    // Navigation: Package - SampleMatrix (nền mẫu)
    public SampleMatrix? SampleMatrix { get; set; }
    public Account? UpdatedByAccount { get; set; }
}

