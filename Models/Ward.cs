namespace VietLab.Models;

public class Ward
{
    public Guid WardId { get; set; }
    public int? SequenceNumber { get; set; } // STT
    public string? Code { get; set; } // Mã
    public string Name { get; set; } = string.Empty; // Xã/Phường
    public string? Type { get; set; } // Loại (Xã, Phường, Thị trấn)
    public Guid ProvinceId { get; set; } // Tỉnh/Thành Phố
    public Guid CountryId { get; set; } // Quốc Gia
    public string Status { get; set; } = "Active"; // Trạng Thái
    public string? Notes { get; set; } // Ghi chú

    // Navigation properties
    public Province? Province { get; set; }
    public Country? Country { get; set; }
}

