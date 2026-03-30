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

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }    // AccountId người cập nhật

    // Navigation
    public Branch? Branch { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
}


