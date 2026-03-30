namespace VietLab.Models;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; } // Code nhân viên
    public Guid? DepartmentId { get; set; }   // FK phòng ban
    public Guid? SectionId { get; set; }      // FK bộ phận
    public string? Role { get; set; }         // Quyền/Role
    public string FullName { get; set; } = string.Empty;     // Tên nhân viên
    public Guid? EmployeeTitleId { get; set; } // FK đến danh mục chức vụ
    public string? Title { get; set; }        // Chức vụ (giữ để tương thích / nhập tự do)
    public string? Email { get; set; }
    public string? ExtensionNumber { get; set; }  // Số máy nhánh
    public string? Mobile { get; set; }       // SĐT
    public string? Notes { get; set; }        // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";           // Trạng thái

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }    // AccountId người cập nhật

    // Quan hệ manager (self-referencing)
    public Guid? ManagerId { get; set; }      // Foreign key đến Employee (manager)

    // Navigation
    public Department? Department { get; set; }
    public Section? Section { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public Account? Account { get; set; }
    public Employee? Manager { get; set; }   // Manager của employee này
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>(); // Các nhân viên dưới quyền
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    public EmployeeTitle? EmployeeTitle { get; set; } // Chức vụ (danh mục)
}


