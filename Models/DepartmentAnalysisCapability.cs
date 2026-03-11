namespace VietLab.Models;

public class DepartmentAnalysisCapability
{
    public Guid DepartmentAnalysisCapabilityId { get; set; }
    public Guid DepartmentId { get; set; }
    public string BranchId { get; set; } = string.Empty; // nvarchar(50) - derived from department.branch_id
    public Guid AnalysisItemId { get; set; }
    public bool Nd107 { get; set; }
    public DateTime? Nd107ExpiredDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation Properties
    public Department? Department { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public ICollection<DepartmentAnalysisCapabilityDesignation> Designations { get; set; } = new List<DepartmentAnalysisCapabilityDesignation>();
}

