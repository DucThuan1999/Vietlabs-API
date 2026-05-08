namespace VietLab.Models;

public class OrderHistory
{
    public Guid OrderHistoryId { get; set; }
    public Guid OrderId { get; set; } // Foreign key đến Order
    public DateTime ActivityDate { get; set; } // (VN: Ngày)
    public string? Activity { get; set; } // (VN: Hoạt động)
    public string? Notes { get; set; } // (VN: Ghi chú)
    public string? Status { get; set; } // (VN: Trạng thái)
    public Guid CreatedByAccountId { get; set; } // (VN: Người thực hiện)

    // Navigation properties
    public Order? Order { get; set; }
    public Account? CreatedByAccount { get; set; }
}

