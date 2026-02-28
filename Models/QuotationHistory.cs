namespace VietLab.Models;

public class QuotationHistory
{
    public Guid QuotationHistoryId { get; set; }
    public Guid QuotationId { get; set; }
    public DateTime ChangedDate { get; set; }
    public string ChangeDescription { get; set; } = string.Empty; // Nội dung thay đổi
    public Guid ChangedByAccountId { get; set; } // User nào thay đổi
    public string? ChangeType { get; set; } // Created, Updated, Deleted, StatusChanged, Approved, Rejected (optional)
    
    // Có thể lưu snapshot dữ liệu cũ (JSON) để so sánh
    public string? OldValues { get; set; } // JSON snapshot của dữ liệu cũ
    public string? NewValues { get; set; } // JSON snapshot của dữ liệu mới

    // Navigation properties
    public Quotation? Quotation { get; set; }
    public Account? ChangedByAccount { get; set; }
}

