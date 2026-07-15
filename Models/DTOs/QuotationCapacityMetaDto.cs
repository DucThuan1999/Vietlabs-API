namespace VietLab.Models.DTOs;

/// <summary>
/// Năng lực/chỉ định (NĐ107, designations) đã join + gom nhóm theo AnalysisItemId ở backend —
/// thay cho việc FE tải riêng 4 bảng (DepartmentAnalysisCapabilities/Designations,
/// SubcontractorCapabilities/Designations) rồi tự join bằng JS. Dữ liệu chung cho mọi báo giá.
/// </summary>
public class QuotationCapacityMetaResponse
{
    public List<Guid> AnalysisItemIdsWithSubcontractorCapacity { get; set; } = new();
    public List<Guid> AnalysisItemIdsWithNd107Vietlabs { get; set; } = new();
    public List<Guid> AnalysisItemIdsWithNd107Subcontractor { get; set; } = new();
    public Dictionary<string, List<string>> DesignationsByAnalysisItemVietlabs { get; set; } = new();
    public Dictionary<string, List<string>> DesignationsByAnalysisItemSubcontractor { get; set; } = new();
    public Dictionary<string, List<string>> DesignationSymbolsByDepartmentCapabilityId { get; set; } = new();
    public Dictionary<string, List<string>> DesignationSymbolsBySubcontractorCapabilityId { get; set; } = new();
    public Dictionary<string, List<string>> DesignationSymbolsByAnalysisItemIdVietlabs { get; set; } = new();
    public Dictionary<string, List<string>> DesignationSymbolsByAnalysisItemIdSubcontractor { get; set; } = new();
}
