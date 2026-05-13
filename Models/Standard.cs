namespace VietLab.Models;

/// <summary>
/// Danh mục Quy chuẩn/Tiêu chuẩn
/// </summary>
public class Standard
{
    public Guid StandardId { get; set; }
    public int? SequenceNumber { get; set; }  // STT
    public string? StandardCode { get; set; }  // Mã quy chuẩn/tiêu chuẩn
    public string? NameVi { get; set; }  // Quy chuẩn/Tiêu chuẩn (VIE)
    public string? NameEn { get; set; }  // Tên quy chuẩn/tiêu chuẩn (ENG)
    public string Status { get; set; } = "Active";  // Trạng thái
    public string? Notes { get; set; }  // Ghi chú
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
}
