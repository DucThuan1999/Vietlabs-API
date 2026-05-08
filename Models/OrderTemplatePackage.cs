namespace VietLab.Models;

/// <summary>
/// Gói phân tích template gắn với mẫu đơn hàng — cấu trúc giống OrderSamplePackage.
/// </summary>
public class OrderTemplatePackage
{
    public Guid OrderTemplatePackageId { get; set; }
    public Guid TemplateId { get; set; }

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

    public ICollection<OrderTemplatePackageAnalysisItem> OrderTemplatePackageAnalysisItems { get; set; } = new List<OrderTemplatePackageAnalysisItem>();

    public OrderTemplate? OrderTemplate { get; set; }
    public SampleMatrix? SampleMatrix { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
