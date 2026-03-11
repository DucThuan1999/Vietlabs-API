namespace VietLab.Models;

/// <summary>
/// Danh mục Phương pháp tham chiếu: mapping tên phương pháp với mã tham chiếu quốc tế.
/// </summary>
public class ReferenceMethod
{
    public Guid ReferenceMethodId { get; set; }
    public int? SequenceNumber { get; set; }  // STT
    public string? ReferenceMethodCode { get; set; }  // Mã phương pháp
    public string? NameVi { get; set; }  // Tên phương pháp (VIE)
    public string? NameEn { get; set; }  // Tên phương pháp (ENG)
    public string Status { get; set; } = "Active";  // Trạng thái
    public string? Notes { get; set; }  // Ghi chú
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // Người cập nhật (FK -> account)

    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
}
