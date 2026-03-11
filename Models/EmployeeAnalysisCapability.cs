namespace VietLab.Models;

/// <summary>
/// Năng lực nhân viên - gắn nhân viên với chỉ tiêu phân tích mà nhân viên được chỉ định thực hiện.
/// </summary>
public class EmployeeAnalysisCapability
{
    public Guid EmployeeAnalysisCapabilityId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid AnalysisItemId { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public Employee? Employee { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
