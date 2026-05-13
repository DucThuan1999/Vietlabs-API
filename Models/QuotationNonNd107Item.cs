namespace VietLab.Models;

/// <summary>
/// Ghi nhận chỉ tiêu chưa có NĐ107 trên báo giá — không tính tiền, không in PDF.
/// </summary>
public class QuotationNonNd107Item
{
    public Guid QuotationNonNd107ItemId { get; set; }
    public Guid QuotationId { get; set; }

    /// <summary>Criterion | Group | Package</summary>
    public string SourceType { get; set; } = string.Empty;

    public Guid? AnalysisItemId { get; set; }
    public Guid? AnalysisGroupId { get; set; }
    public Guid? PackageId { get; set; }

    public string? ItemCode { get; set; }
    public string? ItemNameVi { get; set; }
    public string? ItemNameEn { get; set; }
    public string? SampleMatrixName { get; set; }
    public string? ReferenceMethodCode { get; set; }
    public string? ReferenceMethodNameVi { get; set; }
    public string? Unit { get; set; }
    public string? Lod { get; set; }
    public string? Loq { get; set; }
    public string? Tat { get; set; }
    public string? Notes { get; set; }
    public int? DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Quotation? Quotation { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Package? Package { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
