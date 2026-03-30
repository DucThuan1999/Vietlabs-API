namespace VietLab.Models;

public class Section
{
    public Guid SectionId { get; set; }
    public string? SectionCode { get; set; } // Code Bộ phận
    public Guid DepartmentId { get; set; }   // Thuộc phòng ban nào
    public string? NameVi { get; set; }      // Tên Bộ phận (VIE)
    public string? NameEn { get; set; }      // Tên Bộ phận (ENG)
    public string? Notes { get; set; }       // Mô tả/Ghi chú
    public string Status { get; set; } = "Active"; // Trạng thái

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }     // AccountId người cập nhật

    // Navigation
    public Department? Department { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
