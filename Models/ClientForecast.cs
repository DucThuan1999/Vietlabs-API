namespace VietLab.Models;

/// <summary>
/// Forecast của khách hàng (1-n với Client, một client có nhiều forecast theo thời gian)
/// </summary>
public class ClientForecast
{
    public Guid ClientForecastId { get; set; }
    public Guid ClientId { get; set; } // Foreign key đến Client

    // Thông tin Forecast
    public DateTime FromDate { get; set; } // Từ ngày
    public DateTime ToDate { get; set; } // Đến ngày
    public decimal ForecastAmount { get; set; } // Forecast (số tiền)
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; } // AccountId của người tạo
    public Guid? UpdatedBy { get; set; } // AccountId của người cập nhật

    // Navigation
    public Client? Client { get; set; }
    public Account? CreatedByAccount { get; set; } // Thông tin người tạo
    public Account? UpdatedByAccount { get; set; } // Thông tin người cập nhật
}

