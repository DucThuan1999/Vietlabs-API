namespace VietLab.Models;

public class ClientHistory
{
    public Guid ClientHistoryId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime ChangedDate { get; set; }
    public string ChangeDescription { get; set; } = string.Empty; // Nội dung thay đổi
    public Guid ChangedByAccountId { get; set; } // User nào thay đổi
    public string? ChangeType { get; set; } // Created, Updated, Deleted (optional)

    // Navigation properties
    public Client? Client { get; set; }
    public Account? ChangedByAccount { get; set; }
}

