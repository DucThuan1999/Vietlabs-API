namespace VietLab.Models;

public class Department
{
    public Guid DepartmentId { get; set; }
    public string DepartmentCode { get; set; } = string.Empty; // Code Phòng Ban
    public Guid BranchId { get; set; }                         // Thuộc chi nhánh nào
    public string NameVi { get; set; } = string.Empty;         // Tên Phòng Ban (VIE)
    public string NameEn { get; set; } = string.Empty;         // Tên Phòng Ban (ENG)
    public string Notes { get; set; } = string.Empty;          // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";             // Trạng thái

    // Navigation
    public Branch? Branch { get; set; }
}


