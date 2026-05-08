namespace VietLab.Models;

/// <summary>
/// Chỉ tiêu / dòng template gắn với mẫu đơn hàng — cấu trúc giống OrderSampleItem.
/// </summary>
public class OrderTemplateItem
{
    public Guid OrderTemplateItemId { get; set; }
    public Guid TemplateId { get; set; }

    public string ItemType { get; set; } = string.Empty;

    public Guid? AnalysisItemId { get; set; }
    public Guid? AnalysisGroupId { get; set; }
    public Guid? PackageId { get; set; }

    public bool? IsStandalone { get; set; }

    public string? CapacityType { get; set; }
    public Guid? DepartmentAnalysisCapabilityId { get; set; }
    public Guid? SubcontractorCapabilityId { get; set; }

    public string? ItemCode { get; set; }
    public string? ItemNameVi { get; set; }
    public string? ItemNameEn { get; set; }
    public string? Description { get; set; }

    public string? SampleMatrixName { get; set; }
    public string? PublishedGroupCode { get; set; }
    public string? Unit { get; set; }
    public string? Lod { get; set; }
    public string? Loq { get; set; }
    public string? Tat { get; set; }

    public int Quantity { get; set; } = 1;
    public decimal? DefaultPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal SubTotal { get; set; }

    public int? DisplayOrder { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public OrderTemplate? OrderTemplate { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Package? Package { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public DepartmentAnalysisCapability? DepartmentAnalysisCapability { get; set; }
    public SubcontractorCapability? SubcontractorCapability { get; set; }
}
