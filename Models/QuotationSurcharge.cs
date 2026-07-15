namespace VietLab.Models;

/// <summary>
/// Phụ phí báo giá — cộng một lần vào tạm tính, không nhân theo số mẫu.
/// </summary>
public class QuotationSurcharge
{
    public Guid QuotationSurchargeId { get; set; }
    public Guid QuotationId { get; set; }

    /// <summary>
    /// Transportation | PrintResult | SamplingLabor | SamplingTools | Other
    /// </summary>
    public string SurchargeType { get; set; } = string.Empty;

    /// <summary>Mô tả hiển thị; bắt buộc khi SurchargeType = Other.</summary>
    public string? Description { get; set; }

    public decimal Amount { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Quotation? Quotation { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
