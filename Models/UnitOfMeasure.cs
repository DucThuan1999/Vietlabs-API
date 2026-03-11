namespace VietLab.Models;

/// <summary>
/// Danh mục Đơn vị tính (ĐVT): dùng cho AnalysisItem.UnitOfMeasureId.
/// </summary>
public class UnitOfMeasure
{
    public Guid UnitOfMeasureId { get; set; }
    public int? SequenceNumber { get; set; }  // STT
    public string? UnitOfMeasureCode { get; set; }  // Mã đơn vị tính
    public string? NameVi { get; set; }  // Tên (VIE)
    public string? NameEn { get; set; }  // Tên (ENG)
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
}
