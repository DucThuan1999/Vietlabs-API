namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: DepartmentAnalysisCapability - Designation (many-to-many).
/// Một năng lực phòng ban có nhiều chỉ định, mỗi bản ghi có ngày hết hạn.
/// </summary>
public class DepartmentAnalysisCapabilityDesignation
{
    public Guid DepartmentAnalysisCapabilityDesignationId { get; set; }
    public Guid DepartmentAnalysisCapabilityId { get; set; }
    public Guid DesignationId { get; set; }
    public DateTime? ExpiredDate { get; set; }

    public DepartmentAnalysisCapability? DepartmentAnalysisCapability { get; set; }
    public Designation? Designation { get; set; }
}
