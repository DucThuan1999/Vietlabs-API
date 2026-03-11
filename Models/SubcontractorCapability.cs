namespace VietLab.Models;

/// <summary>
/// Năng lực nhà thầu phụ - mapping giữa nhà thầu phụ và chỉ tiêu (AnalysisItem)
/// </summary>
public class SubcontractorCapability
{
    public Guid SubcontractorCapabilityId { get; set; }
    /// <summary>FK - Nhà thầu phụ</summary>
    public Guid SubcontractorId { get; set; }
    /// <summary>FK - Chỉ tiêu phân tích</summary>
    public Guid AnalysisItemId { get; set; }
    public bool Nd107 { get; set; }
    public DateTime? Nd107ExpiredDate { get; set; }
    /// <summary>Mô tả / Ghi chú</summary>
    public string? Notes { get; set; }
    /// <summary>Trạng thái (Active, Inactive, ...)</summary>
    public string Status { get; set; } = "Active";

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Subcontractor? Subcontractor { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public ICollection<SubcontractorCapabilityDesignation> Designations { get; set; } = new List<SubcontractorCapabilityDesignation>();
}
