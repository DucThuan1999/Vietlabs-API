namespace VietLab.Models;

public class DepartmentAnalysisCapability
{
    public Guid DepartmentAnalysisCapabilityId { get; set; }
    public Guid DepartmentId { get; set; }
    public string BranchId { get; set; } = string.Empty; // nvarchar(50) - derived from department.branch_id
    public Guid AnalysisItemId { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Department? Department { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
}

