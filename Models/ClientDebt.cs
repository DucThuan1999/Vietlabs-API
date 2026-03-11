namespace VietLab.Models;

/// <summary>
/// Công nợ của khách hàng (1-1 với Client, chỉ lưu công nợ latest từ hệ thống kế toán MISA)
/// </summary>
public class ClientDebt
{
    public Guid ClientDebtId { get; set; }
    public Guid ClientId { get; set; } // Foreign key đến Client (1-1)

    // Thông tin công nợ
    public string? PaymentMethod { get; set; } // Hình thức thanh toán
    public decimal TotalDebt { get; set; } // Tổng công nợ
    public int DebtTermDays { get; set; } // Thời hạn công nợ (số ngày)
    public decimal CreditLimit { get; set; } // Hạn mức dư nợ
    
    // Thông tin liên hệ công nợ
    public string? DebtContactName { get; set; } // Người liên lạc công nợ
    public string? DebtContactPhone { get; set; } // SĐT liên lạc công nợ
    public string? DebtContactEmail { get; set; } // Email liên lạc công nợ
    
    // Tình trạng hợp đồng
    public DateTime? ContractEffectiveDate { get; set; } // Tình trạng hợp đồng hiệu lực ngày
    public DateTime? ContractEndDate { get; set; } // Tình trạng hợp đồng kết thúc ngày
    
    // Attachments (có thể lưu JSON array hoặc string paths)
    public string? Attachments { get; set; } // JSON array hoặc comma-separated paths

    // Thông tin sync từ MISA
    public DateTime? LastSyncedAt { get; set; } // Thời gian sync lần cuối từ MISA
    public string? MisaReferenceId { get; set; } // ID tham chiếu từ hệ thống MISA

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation
    public Client? Client { get; set; }
    public Account? UpdatedByAccount { get; set; }
}

