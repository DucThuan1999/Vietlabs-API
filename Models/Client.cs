namespace VietLab.Models;

public class Client
{
    public Guid ClientId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string InternalCode { get; set; } = string.Empty; // Mã khách hàng nội bộ
    public string TaxCode { get; set; } = string.Empty; // Mã số thuế
    public string BankAccountNumber { get; set; } = string.Empty; // Số tài khoản
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty; // Ngành nghề
    public string Scale { get; set; } = string.Empty; // Quy mô
    public string CustomerType { get; set; } = string.Empty; // Loại khách hàng
    public decimal DiscountRate { get; set; } // Mức chiết khấu (%)

    // Người đại diện
    public string RepresentativeName { get; set; } = string.Empty;
    public string RepresentativeEmail { get; set; } = string.Empty;
    public string RepresentativePhone { get; set; } = string.Empty;
    public string RepresentativeTitle { get; set; } = string.Empty;

    // Nhân viên kinh doanh phụ trách
    public string SalesOwnerName { get; set; } = string.Empty;
    public string SalesOwnerEmail { get; set; } = string.Empty;
    public string SalesOwnerPhone { get; set; } = string.Empty;

    public bool IsBlacklisted { get; set; }
    public string BlacklistReason { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? LastContactDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Prospect
    public string Notes { get; set; } = string.Empty;

    // Navigation: mỗi khách hàng có nhiều người liên hệ
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

