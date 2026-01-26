namespace VietLab.Models;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; } // Code nhân viên
    public string? Department { get; set; }   // Phòng ban
    public string? Role { get; set; }         // Quyền/Role
    public string FullName { get; set; } = string.Empty;     // Tên nhân viên
    public string? Title { get; set; }        // Chức vụ
    public string? Email { get; set; }
    public string? Notes { get; set; }        // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";           // Trạng thái

    // Navigation
    public Account? Account { get; set; }
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}


