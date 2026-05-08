namespace VietLab.Models;

/// <summary>
/// Nhóm chỉ tiêu template gắn với mẫu đơn hàng — cấu trúc giống OrderSampleAnalysisGroup.
/// </summary>
public class OrderTemplateAnalysisGroup
{
    public Guid OrderTemplateAnalysisGroupId { get; set; }
    public Guid TemplateId { get; set; }
    public Guid AnalysisGroupId { get; set; }

    public decimal? StepPrice { get; set; }
    public decimal? GroupSalePrice { get; set; }
    public decimal? DiscountRate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public OrderTemplate? OrderTemplate { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
