namespace VietLab.Models;

public class AnalysisItemTat
{
    public Guid AnalysisItemTatId { get; set; }
    public Guid AnalysisItemId { get; set; }
    public string TatType { get; set; } = string.Empty; // "Normal", "Fast", "Urgent" hoặc "Thường", "Nhanh", "Khẩn"
    public int TatValue { get; set; } // Giá trị TAT (số ngày hoặc giờ)
    public string TatUnit { get; set; } = "Days"; // "Days" hoặc "Hours"
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public AnalysisItem? AnalysisItem { get; set; }
}

