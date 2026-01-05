namespace VietLab.Models;

public class AnalysisGroup
{
    public Guid AnalysisGroupId { get; set; }
    public string? AnalysisGroupCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation: 1 AnalysisGroup có nhiều AnalysisItem
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
}

