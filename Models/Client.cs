namespace VietLab.Models;

public class Client
{
    public Guid ClientId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? InternalCode { get; set; } // Mã khách hàng nội bộ
    public string? TaxCode { get; set; } // Mã số thuế
    public string? BankAccountNumber { get; set; } // Số tài khoản
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Profession { get; set; } // Ngành nghề
    public string? Scale { get; set; } // Quy mô
    public string? CustomerType { get; set; } // Loại khách hàng
    public decimal DiscountRate { get; set; } // Mức chiết khấu (%)

    // Người đại diện
    public string? RepresentativeName { get; set; }
    public string? RepresentativeEmail { get; set; }
    public string? RepresentativePhone { get; set; }
    public string? RepresentativeTitle { get; set; }

    // Nhân viên kinh doanh phụ trách
    public string? SalesOwnerName { get; set; }
    public string? SalesOwnerEmail { get; set; }
    public string? SalesOwnerPhone { get; set; }

    public bool IsBlacklisted { get; set; }
    public string? BlacklistReason { get; set; }

    // Thông tin bổ sung cho báo giá
    public string? AgentName { get; set; } // Tên Đại lý
    public decimal? Forecast { get; set; } // Forcast
    public decimal? Revenue { get; set; } // Doanh thu

    // Thông tin công nợ
    public string? DebtContactName { get; set; } // Người liên lạc công nợ
    public string? DebtContactPhone { get; set; } // SĐT liên lạc công nợ
    public string? DebtContactEmail { get; set; } // Email liên lạc công nợ
    public string? PaymentMethod { get; set; } // Hình thức thanh toán

    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Prospect
    public string? Notes { get; set; }

    // Navigation: mỗi khách hàng có nhiều người liên hệ
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    // Navigation: mỗi khách hàng có nhiều báo giá
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    // Navigation: mỗi khách hàng có 1 công nợ latest
    public ClientDebt? ClientDebt { get; set; }
    // Navigation: mỗi khách hàng có nhiều forecast
    public ICollection<ClientForecast> ClientForecasts { get; set; } = new List<ClientForecast>();
}

