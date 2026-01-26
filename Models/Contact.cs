namespace VietLab.Models;

public class Contact
{
    public Guid ContactId { get; set; }
    public Guid ClientId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Title { get; set; } // Chức vụ
    public bool IsPrimary { get; set; }
    public string? Notes { get; set; } // Ghi chú
    
    // Vai trò người liên hệ
    public bool IsSampleSender { get; set; } // Người gửi mẫu
    public bool IsResultReceiver { get; set; } // Người nhận kết quả
    public bool IsPayer { get; set; } // Người thanh toán

    // Navigation
    public Client? Client { get; set; }
}


