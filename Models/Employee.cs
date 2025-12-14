namespace VietLab.Models;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty; // Code nhân viên
    public string Department { get; set; } = string.Empty;   // Phòng ban
    public string Role { get; set; } = string.Empty;         // Quyền/Role
    public string FullName { get; set; } = string.Empty;     // Tên nhân viên
    public string Title { get; set; } = string.Empty;        // Chức vụ
    public string Email { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;        // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";           // Trạng thái

    // Navigation
    public Account? Account { get; set; }
}


