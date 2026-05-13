namespace VietLab.Models;

public class Quotation
{
    public Guid QuotationId { get; set; }
    public string? QuotationCode { get; set; } // Mã báo giá
    /// <summary>Tiêu đề hiển thị trên báo giá / báo cáo in.</summary>
    public string? QuotationTitle { get; set; }

    // THÔNG TIN NHÂN VIÊN KINH DOANH (NGƯỜI TẠO BÁO GIÁ)
    public Guid? EmployeeId { get; set; } // Foreign key đến Employee
    public string? SalesPersonName { get; set; } // Tên nhân viên kinh doanh
    public string? SalesPersonEmail { get; set; } // Email nhân viên kinh doanh
    public string? SalesPersonPhone { get; set; } // SĐT nhân viên kinh doanh

    // THÔNG TIN KHÁCH HÀNG
    public Guid ClientId { get; set; } // Foreign key đến Client
    public string? AgentName { get; set; } // Tên Đại lý (có thể lấy từ Client hoặc override)
    public string? CompanyName { get; set; } // Tên Cty/Doanh nghiệp (có thể lấy từ Client hoặc override)
    public Guid? ContactId { get; set; } // Foreign key đến Contact (Người liên hệ)
    public string? ContactName { get; set; } // Người liên hệ (có thể lấy từ Contact hoặc override)
    public string? TaxCode { get; set; } // Mã số thuế (có thể lấy từ Client hoặc override)
    public string? ContactEmail { get; set; } // Email người liên hệ (có thể lấy từ Contact hoặc override)
    public decimal? Forecast { get; set; } // Forcast (có thể lấy từ Client hoặc override)
    public string? ContactPhone { get; set; } // SĐT người liên hệ (có thể lấy từ Contact hoặc override)
    public decimal? Revenue { get; set; } // Doanh thu (có thể lấy từ Client hoặc override)
    public string? Address { get; set; } // Địa chỉ (có thể lấy từ Client hoặc override)

    // THÔNG TIN CÔNG NỢ
    public string? DebtContactName { get; set; } // Người liên lạc công nợ (có thể lấy từ Client hoặc override)
    public string? DebtContactPhone { get; set; } // SĐT liên lạc công nợ (có thể lấy từ Client hoặc override)
    public string? DebtContactEmail { get; set; } // Email liên lạc công nợ (có thể lấy từ Client hoặc override)
    public string? PaymentMethod { get; set; } // Hình thức thanh toán (có thể lấy từ Client hoặc override)

    // HIỆU LỰC BÁO GIÁ
    public DateTime? ValidFrom { get; set; } // Từ ngày
    public DateTime? ValidTo { get; set; } // Đến ngày

    // GIẢM GIÁ
    public decimal? DiscountPercent { get; set; } // Giảm giá %
    public decimal? DiscountAmount { get; set; } // Số tiền giảm giá

    // TÓM TẮT
    public decimal? SubTotal { get; set; } // Tạm tính
    public decimal? TotalDiscountPercent { get; set; } // Tổng % giảm giá
    public decimal? TotalDiscount { get; set; } // Tổng giảm
    public decimal? VatPercent { get; set; } = 8; // VAT % (mặc định 8%)
    public decimal? VatAmount { get; set; } // VAT 8% (số tiền)
    public decimal? TotalAmount { get; set; } // Tổng Đơn giá

    // CHIẾT KHẤU
    public decimal? QuotationDiscountPercent { get; set; } // % theo báo giá
    public decimal? ClientDiscountPercent { get; set; } // Theo khách hàng (load từ table khách hàng - DiscountRate)

    // Thông tin chung
    public string Status { get; set; } = "Draft"; // Draft, Sent, Approved, Rejected, Expired
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; } // AccountId của người tạo
    public Guid? UpdatedBy { get; set; } // AccountId của người cập nhật

    // PHÊ DUYỆT BÁO GIÁ
    public Guid? ApproverLevel1Id { get; set; } // Người phê duyệt cấp 1 (Manager của Employee)
    public Guid? ApproverLevel2Id { get; set; } // Người phê duyệt cấp 2 (Người chỉ định)
    public DateTime? ApprovedLevel1At { get; set; } // Thời gian phê duyệt cấp 1
    public DateTime? ApprovedLevel2At { get; set; } // Thời gian phê duyệt cấp 2
    public string? ApprovalLevel1Status { get; set; } // Pending, Approved, Rejected
    public string? ApprovalLevel2Status { get; set; } // Pending, Approved, Rejected
    public string? ApprovalLevel1Comment { get; set; } // Ghi chú phê duyệt cấp 1
    public string? ApprovalLevel2Comment { get; set; } // Ghi chú phê duyệt cấp 2

    // Navigation Properties
    public Employee? Employee { get; set; }
    public Employee? ApproverLevel1 { get; set; } // Navigation đến Employee (Manager)
    public Employee? ApproverLevel2 { get; set; } // Navigation đến Employee (Người chỉ định)
    public Client? Client { get; set; }
    public Contact? Contact { get; set; }
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
    public ICollection<QuotationNonNd107Item> QuotationNonNd107Items { get; set; } = new List<QuotationNonNd107Item>();
    public ICollection<QuotationSample> QuotationSamples { get; set; } = new List<QuotationSample>();
    public ICollection<QuotationAnalysisGroup> QuotationAnalysisGroups { get; set; } = new List<QuotationAnalysisGroup>();
    public ICollection<QuotationHistory> QuotationHistories { get; set; } = new List<QuotationHistory>();
}

