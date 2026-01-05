namespace VietLab.Models;

public class Department
{
    public Guid DepartmentId { get; set; }
    public string? DepartmentCode { get; set; } // Code Phòng Ban
    public Guid BranchId { get; set; }                         // Thuộc chi nhánh nào
    public string? NameVi { get; set; }         // Tên Phòng Ban (VIE)
    public string? NameEn { get; set; }         // Tên Phòng Ban (ENG)
    public string? Notes { get; set; }          // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";             // Trạng thái

    // Navigation
    public Branch? Branch { get; set; }
}


