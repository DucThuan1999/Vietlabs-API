namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: SubcontractorCapability - Designation (many-to-many).
/// Một năng lực nhà thầu phụ có nhiều chỉ định, mỗi bản ghi có ngày hết hạn.
/// </summary>
public class SubcontractorCapabilityDesignation
{
    public Guid SubcontractorCapabilityDesignationId { get; set; }
    public Guid SubcontractorCapabilityId { get; set; }
    public Guid DesignationId { get; set; }
    public DateTime? ExpiredDate { get; set; }

    public SubcontractorCapability? SubcontractorCapability { get; set; }
    public Designation? Designation { get; set; }
}
