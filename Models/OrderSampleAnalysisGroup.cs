namespace VietLab.Models;

/// <summary>
/// Nhóm chỉ tiêu trong ngữ cảnh mẫu đơn hàng — cấu trúc giống QuotationAnalysisGroup.
/// </summary>
public class OrderSampleAnalysisGroup
{
    public Guid OrderSampleAnalysisGroupId { get; set; }
    public Guid OrderSampleId { get; set; }
    public Guid AnalysisGroupId { get; set; }

    public decimal? StepPrice { get; set; }
    public decimal? GroupSalePrice { get; set; }
    public decimal? DiscountRate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public OrderSample? OrderSample { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
