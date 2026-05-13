namespace VietLab.Models;

/// <summary>
/// Danh mục Chỉ định
/// </summary>
public class Designation
{
    public Guid DesignationId { get; set; }
    public int? SequenceNumber { get; set; }  // STT
    public string? DesignationCode { get; set; }  // Mã chỉ định
    /// <summary>Ký hiệu in PDF sau cột Phương pháp, ví dụ a → (a). Không bắt buộc phải có ngoặc.</summary>
    public string? SymbolCode { get; set; }
    public string? Name { get; set; }  // Tên chỉ định
    /// <summary>Mô tả (bilingual) hiển thị trong chú giải / quản trị.</summary>
    public string? Description { get; set; }
    public string? Note { get; set; }  // Ghi chú
    public string Status { get; set; } = "Active";  // Trạng thái
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItemDesignation> AnalysisItemDesignations { get; set; } = new List<AnalysisItemDesignation>();
    public ICollection<DepartmentAnalysisCapabilityDesignation> DepartmentAnalysisCapabilityDesignations { get; set; } = new List<DepartmentAnalysisCapabilityDesignation>();
    public ICollection<SubcontractorCapabilityDesignation> SubcontractorCapabilityDesignations { get; set; } = new List<SubcontractorCapabilityDesignation>();
}
