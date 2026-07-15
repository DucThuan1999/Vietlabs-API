namespace VietLab.Models;

/// <summary>
/// Nội dung hiển thị ở chân trang PDF báo giá (\"Thông tin ban hành\"), theo khoảng ngày hiệu lực.
/// </summary>
public class QuotationIssueInfo
{
    public Guid QuotationIssueInfoId { get; set; }

    /// <summary>
    /// Chuỗi hiển thị nguyên văn (ví dụ: VLAB01.KD   Lần BH: 02    Ngày BH: 05/05/2022).
    /// </summary>
    public string Content { get; set; } = "";

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
}
