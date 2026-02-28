namespace VietLab.Models;

public class Province
{
    public Guid ProvinceId { get; set; }
    public string? ProvinceCode { get; set; }
    public int? SequenceNumber { get; set; } // STT
    public string Name { get; set; } = string.Empty; // Tỉnh/Thành phố
    public string? Type { get; set; } // Loại (Tỉnh, Thành phố, Thành phố trực thuộc TW)
    public string? FullName { get; set; } // Đầy đủ
    public Guid CountryId { get; set; } // Quốc Gia
    public string Status { get; set; } = "Active"; // Trạng Thái
    public string? Notes { get; set; } // Ghi chú

    // Navigation properties
    public Country? Country { get; set; }
    public ICollection<Ward> Wards { get; set; } = new List<Ward>();
}

