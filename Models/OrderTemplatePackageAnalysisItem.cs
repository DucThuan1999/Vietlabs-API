namespace VietLab.Models;

/// <summary>
/// Chỉ tiêu trong gói template — cấu trúc giống OrderSamplePackageAnalysisItem (con OrderTemplatePackage).
/// </summary>
public class OrderTemplatePackageAnalysisItem
{
    public Guid OrderTemplatePackageAnalysisItemId { get; set; }
    public Guid OrderTemplatePackageId { get; set; }
    public Guid AnalysisItemId { get; set; }

    public int? DisplayOrder { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public OrderTemplatePackage? OrderTemplatePackage { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
