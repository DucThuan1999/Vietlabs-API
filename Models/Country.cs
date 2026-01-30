namespace VietLab.Models;

public class Country
{
    public Guid CountryId { get; set; }
    public int? SequenceNumber { get; set; } // STT
    public string NameEn { get; set; } = string.Empty; // Tên nước (EN)
    public string FullNameVi { get; set; } = string.Empty; // Tên đầy đủ (VI)
    public string FullNameEn { get; set; } = string.Empty; // Tên đầy đủ (EN)
    public string? Alpha2 { get; set; } // Alpha-2 code (VD: VN, US)
    public string? Alpha3 { get; set; } // Alpha-3 code (VD: VNM, USA)
    public string Status { get; set; } = "Active"; // Trạng Thái
    public string? Notes { get; set; } // Ghi chú

    // Navigation properties
    public ICollection<Province> Provinces { get; set; } = new List<Province>();
}

