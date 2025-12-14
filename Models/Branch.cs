namespace VietLab.Models;

public class Branch
{
    public Guid BranchId { get; set; }
    public string BranchCode { get; set; } = string.Empty; // Code chi nhánh
    public string NameVi { get; set; } = string.Empty;     // Tên chi nhánh (VIE)
    public string NameEn { get; set; } = string.Empty;     // Tên chi nhánh (ENG)
    public string License { get; set; } = string.Empty;    // Chứng nhận hoạt động
    public string Notes { get; set; } = string.Empty;      // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";         // Trạng thái

    // Navigation: 1 Branch có nhiều Department
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}


