namespace VietLab.Models;

/// <summary>
/// Danh mục chức vụ nhân viên
/// </summary>
public class EmployeeTitle
{
    public Guid EmployeeTitleId { get; set; }
    public int? SequenceNumber { get; set; } // STT
    public string TitleCode { get; set; } = string.Empty; // Mã chức vụ
    public string NameVi { get; set; } = string.Empty; // Tên tiếng Việt
    public string? NameEn { get; set; } // Tên tiếng Anh
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }   // AccountId người tạo
    public Guid? UpdatedBy { get; set; }   // AccountId người cập nhật

    // Navigation: nhiều nhân viên có cùng chức vụ
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public Account? CreatedByAccount { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
