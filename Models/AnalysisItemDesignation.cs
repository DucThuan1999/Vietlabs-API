namespace VietLab.Models;

/// <summary>
/// Bảng trung gian: AnalysisItem - Designation (many-to-many).
/// Một chỉ tiêu có thể có nhiều chỉ định, mỗi bản ghi có ngày hết hạn.
/// </summary>
public class AnalysisItemDesignation
{
    public Guid AnalysisItemDesignationId { get; set; }
    public Guid AnalysisItemId { get; set; }
    public Guid DesignationId { get; set; }
    public DateTime? ExpiredDate { get; set; }

    public AnalysisItem? AnalysisItem { get; set; }
    public Designation? Designation { get; set; }
}

