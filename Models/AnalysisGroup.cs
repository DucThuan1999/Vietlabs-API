namespace VietLab.Models;

public class AnalysisGroup
{
    public Guid AnalysisGroupId { get; set; }
    public string? AnalysisGroupCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    /// <summary>Giá nhóm chuẩn — cột Excel <c>Giá nhóm chuẩn_new</c> (sheet Vietlabs / export CSV).</summary>
    public decimal? WholeGroupStandardPrice { get; set; }
    public decimal? StepPrice { get; set; } // Giá bước nhảy
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation: 1 AnalysisGroup có nhiều AnalysisItem
    public ICollection<AnalysisItem> AnalysisItems { get; set; } = new List<AnalysisItem>();
    public Account? UpdatedByAccount { get; set; }
}

