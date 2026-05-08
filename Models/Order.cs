namespace VietLab.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public Guid? ContactId { get; set; } // Người liên hệ (phải thuộc cùng ClientId)

    // THÔNG TIN KHÁCH HÀNG
    public string? CustomerCode { get; set; } // (VN: Mã khách hàng)
    public string? CustomerName { get; set; } // (VN: Tên khách hàng)
    public string? CustomerTaxCode { get; set; } // (VN: Mã số thuế)
    public string? AgentName { get; set; } // (VN: Đại lý)
    public string? CustomerAddress { get; set; } // (VN: Địa chỉ)

    public string? SampleSenderName { get; set; } // (VN: Người gửi mẫu)
    public string? SampleSenderEmail { get; set; } // (VN: Email người gửi mẫu)
    public string? SampleSenderPhone { get; set; } // (VN: Điện thoại người gửi mẫu)

    public string? PayerName { get; set; } // (VN: Người thanh toán)
    public string? PayerEmail { get; set; } // (VN: Email người thanh toán)
    public string? PayerPhone { get; set; } // (VN: Điện thoại người thanh toán)

    public string? IssueInvoice { get; set; } // (VN: Xuất hóa đơn)
    public bool? IsApproved { get; set; } // (VN: IsApproved / Trạng thái duyệt)
    public string? RejectionNote { get; set; } // (VN: Note rejected / Ghi chú từ chối)
    public string? ApprovalNote { get; set; } // (VN: Note approve / Ghi chú duyệt)

    public Guid? QuotationId { get; set; } // (VN: Khóa ngoại bảng Quotation)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo
    public Guid? CreatedBy { get; set; } // AccountId người tạo
    public DateTime? ExpectedCompletionDate { get; set; } // Ngày dự kiến hoàn thành
    public string? DebtType { get; set; } // Loại công nợ
    public string? DebtStatus { get; set; } // Tình trạng công nợ
    public string? OrderStatus { get; set; } // Trạng thái đơn hàng

    public string? TestingPurpose { get; set; } // Mục đích thử nghiệm
    public string? TestMethod { get; set; } // Phương pháp thử nghiệm
    public string? ResultTurnaroundTimeRequirement { get; set; } // Thời gian giao nhận kết quả
    public string? ResultDeliveryChannel { get; set; } // Kênh Trả Kết Quả
    public string? Language { get; set; } // Ngôn ngữ
    public string? Technique { get; set; } // Kĩ thuật
    public string? ComparisonStandard { get; set; } // Tiêu chuẩn so sánh
    public decimal? Total { get; set; } // Tổng
    public decimal? SubtotalBeforeVat { get; set; } // Tổng tiền trước thuế
    public decimal? Vat { get; set; } // Thuế VAT
    public decimal? PaymentAmount { get; set; } // Thanh toán
    public string? SampleReceiptMethod { get; set; } // Hình Thức Nhận Mẫu
    public string? LaboratorySampleRetention { get; set; } // PTN Lưu Mẫu
    public string? AdditionalInformation { get; set; } // Thông tin khác
    public string? TestRequestConfirmation { get; set; } // Xác nhận yêu cầu thử nghiệm
    public string? MailDocumentConfirmation { get; set; } // Xác nhận gửi email
    public string? PaymentMethod { get; set; } // Hình thức thanh toán

    public Client? Client { get; set; }
    public Contact? Contact { get; set; }
    public Quotation? Quotation { get; set; }
    public Account? CreatedByAccount { get; set; }
    public ICollection<OrderSample> OrderSamples { get; set; } = new List<OrderSample>();
    public ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
}
