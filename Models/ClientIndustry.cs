namespace VietLab.Models;

/// <summary>
/// Danh mục ngành nghề khách hàng
/// </summary>
public class ClientIndustry
{
    public Guid ClientIndustryId { get; set; }
    public int? SequenceNumber { get; set; } // STT
    public string IndustryCode { get; set; } = string.Empty; // Mã ngành nghề
    public string NameVi { get; set; } = string.Empty; // Tên tiếng Việt
    public string? NameEn { get; set; } // Tên tiếng Anh
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }   // AccountId người tạo
    public Guid? UpdatedBy { get; set; }   // AccountId người cập nhật

    // Navigation: nhiều khách hàng thuộc một ngành nghề
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public Account? CreatedByAccount { get; set; }
    public Account? UpdatedByAccount { get; set; }
}
