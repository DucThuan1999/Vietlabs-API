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

    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Prospect
    public string? Notes { get; set; }

    // Navigation: mỗi khách hàng có nhiều người liên hệ
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

