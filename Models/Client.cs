namespace VietLab.Models;

public class Client
{
    public Guid ClientId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? CompanyNameEn { get; set; } // Tên công ty tiếng Anh
    public string? InternalCode { get; set; } // Mã khách hàng nội bộ
    public string? TaxCode { get; set; } // Mã số thuế
    public string? BankName { get; set; } // Tên ngân hàng
    public string? BankAccountNumber { get; set; } // Số tài khoản
    public string? BankAccountName { get; set; } // Tên chủ thẻ
    public string? Address { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? Ward { get; set; } // Xã/Phường

    public string? Profession { get; set; } // Ngành nghề
    public string? Scale { get; set; } // Quy mô
    public string? CustomerType { get; set; } // Loại khách hàng
    
    // Giảm giá và hoa hồng
    public decimal? DiscountRate { get; set; } // Giảm giá (%)
    public decimal CommissionRate { get; set; } // Tỷ lệ hoa hồng (%)

    // Người đại diện
    public string? RepresentativeName { get; set; }
    public string? RepresentativeEmail { get; set; }
    public string? RepresentativePhone { get; set; }
    public string? RepresentativeTitle { get; set; }

    // Nhân viên kinh doanh phụ trách
    public string? SalesOwnerName { get; set; }
    public string? SalesOwnerEmail { get; set; }
    public string? SalesOwnerPhone { get; set; }

    // Nhân viên CS (Chăm sóc khách hàng) phụ trách
    public string? CsoOwnerName { get; set; }
    public string? CsoOwnerEmail { get; set; }
    public string? CsoOwnerPhone { get; set; }

    public bool IsBlacklisted { get; set; }
    public string? BlacklistReason { get; set; }

    // Thông tin bổ sung cho báo giá
    public Guid? AgentClientId { get; set; } // Foreign key đến Client có CustomerType = 'Đại lý'
    public decimal? Forecast { get; set; } // Forcast
    public decimal? Revenue { get; set; } // Doanh thu

    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Prospect
    public string? Notes { get; set; }
    public string? IssueInvoice { get; set; }

    // Navigation: mỗi khách hàng có nhiều người liên hệ
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    // Navigation: mỗi khách hàng có nhiều báo giá
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    // Navigation: mỗi khách hàng có 1 công nợ latest
    public ClientDebt? ClientDebt { get; set; }
    // Navigation: mỗi khách hàng có nhiều forecast
    public ICollection<ClientForecast> ClientForecasts { get; set; } = new List<ClientForecast>();
    // Navigation: mỗi khách hàng thuộc về một đại lý (Client có CustomerType = 'Đại lý')
    public Client? AgentClient { get; set; }
    // Navigation: danh sách khách hàng thuộc đại lý này (nếu CustomerType = 'Đại lý')
    public ICollection<Client> AgentClients { get; set; } = new List<Client>();
}

