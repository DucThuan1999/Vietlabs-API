namespace VietLab.Models;

/// <summary>
/// Danh mục Kĩ thuật (Sắc Ký, Cổ Điển, Quang Phổ, Vi Sinh, ...): gắn với AnalysisItem.
/// </summary>
public class LaboratoryTechnique
{
    public Guid LaboratoryTechniqueId { get; set; }
    public int? SequenceNumber { get; set; }
    public string? TechniqueCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
}
