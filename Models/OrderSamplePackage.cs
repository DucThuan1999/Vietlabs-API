namespace VietLab.Models;

/// <summary>
/// Gói phân tích gắn với mẫu đơn hàng — cấu trúc giống Package.
/// </summary>
public class OrderSamplePackage
{
    public Guid OrderSamplePackageId { get; set; }
    public Guid OrderSampleId { get; set; }

    public string? PackageCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public decimal? DefaultPrice { get; set; }
    public string? PublishedGroupCode { get; set; }
    public Guid? SampleMatrixId { get; set; }

    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ICollection<OrderSamplePackageAnalysisItem> OrderSamplePackageAnalysisItems { get; set; } = new List<OrderSamplePackageAnalysisItem>();

    public OrderSample? OrderSample { get; set; }
    public SampleMatrix? SampleMatrix { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
