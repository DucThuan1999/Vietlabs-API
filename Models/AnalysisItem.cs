namespace VietLab.Models;

public class AnalysisItem
{
    public Guid AnalysisItemId { get; set; }
    public string? AnalysisItemCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string? Organization { get; set; }
    
    // Foreign Keys
    public Guid EquipmentTypeId { get; set; }
    public Guid AnalysisGroupId { get; set; }
    public Guid SampleMatrixId { get; set; }
    public Guid SampleMatrixGroupId { get; set; }
    
    public string? PublishedGroupCode { get; set; }
    public decimal? Lod { get; set; }
    public decimal? Loq { get; set; }
    public string? Unit { get; set; }
    
    // Boolean flags
    public bool Nd107 { get; set; }
    public bool Iso { get; set; }
    public bool CucBvtv { get; set; }
    public bool BoCongThuong { get; set; }
    public bool Nafi { get; set; }
    public bool CucChanNuoi { get; set; }
    
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public EquipmentType? EquipmentType { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public SampleMatrix? SampleMatrix { get; set; }
    public SampleMatrixGroup? SampleMatrixGroup { get; set; }
}

