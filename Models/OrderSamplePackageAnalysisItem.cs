namespace VietLab.Models;

/// <summary>
/// Chỉ tiêu trong gói (mẫu đơn hàng) — cấu trúc giống PackageAnalysisItem.
/// </summary>
public class OrderSamplePackageAnalysisItem
{
    public Guid OrderSamplePackageAnalysisItemId { get; set; }
    public Guid OrderSamplePackageId { get; set; }
    public Guid AnalysisItemId { get; set; }

    public int? DisplayOrder { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public OrderSamplePackage? OrderSamplePackage { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
