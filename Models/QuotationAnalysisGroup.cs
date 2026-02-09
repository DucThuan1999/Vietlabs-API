namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: Quotation - AnalysisGroup
/// Lưu thông tin giá (stepPrice và groupSalePrice) của AnalysisGroup trong context của một Quotation cụ thể
/// </summary>
public class QuotationAnalysisGroup
{
    public Guid QuotationAnalysisGroupId { get; set; }
    public Guid QuotationId { get; set; } // Foreign key đến Quotation
    public Guid AnalysisGroupId { get; set; } // Foreign key đến AnalysisGroup

    // Thông tin giá cho AnalysisGroup trong context của Quotation này
    public decimal? StepPrice { get; set; } // Giá bước nhảy (có thể override từ AnalysisGroup.StepPrice)
    public decimal? GroupSalePrice { get; set; } // Giá bán nhóm (có thể override từ AnalysisGroup.WholeGroupStandardPrice)
    public decimal? DiscountRate { get; set; } // Tỷ lệ giảm giá (%)

    // Thông tin bổ sung
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Quotation? Quotation { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
}

