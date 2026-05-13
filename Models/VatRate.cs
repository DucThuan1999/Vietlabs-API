namespace VietLab.Models;

/// <summary>
/// Lịch giá VAT (phần trăm) theo khoảng ngày. EndDate null = hiệu lực không có điểm kết thúc.
/// </summary>
public class VatRate
{
    public Guid VatRateId { get; set; }

    /// <summary>
    /// Phần trăm VAT (ví dụ 8 = 8%).
    /// </summary>
    public decimal Percent { get; set; }

    /// <summary>
    /// Ngày bắt đầu áp dụng (đầu ngày UTC).
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc áp dụng (đầu ngày UTC); null = vô hạn.
    /// </summary>
    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
