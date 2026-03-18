namespace VietLab.Models;

/// <summary>
/// Nhà thầu phụ
/// </summary>
public class Subcontractor
{
    public Guid SubcontractorId { get; set; }
    /// <summary>Mã nhà thầu phụ (NTP-001, NTP-002, ...)</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Short name (giá trị mã cũ trước khi chuyển sang NTP-xxx)</summary>
    public string? ShortName { get; set; }
    /// <summary>Tên nhà thầu</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Người liên hệ</summary>
    public string? ContactPerson { get; set; }
    /// <summary>Số điện thoại</summary>
    public string? Phone { get; set; }
    /// <summary>Email</summary>
    public string? Email { get; set; }
    /// <summary>Địa chỉ</summary>
    public string? Address { get; set; }
    /// <summary>Phòng ban phụ trách (FK)</summary>
    public Guid? DepartmentId { get; set; }
    /// <summary>Mô tả / Ghi chú</summary>
    public string? Notes { get; set; }
    /// <summary>Trạng thái (Active, Inactive, ...)</summary>
    public string Status { get; set; } = "Active";

    /// <summary>Mã số thuế</summary>
    public string? TaxCode { get; set; }
    /// <summary>Số tài khoản ngân hàng</summary>
    public string? BankAccountNumber { get; set; }
    /// <summary>Tên người nhận (tài khoản ngân hàng)</summary>
    public string? BankAccountHolder { get; set; }
    /// <summary>Tên ngân hàng</summary>
    public string? BankName { get; set; }
    /// <summary>Hợp đồng: Yes, No, Overdue</summary>
    public string? ContractStatus { get; set; }
    /// <summary>Chu kỳ thanh toán: BeforeAnalysis, BeforeReceivingResult, AfterInvoice</summary>
    public string? PaymentCycle { get; set; }
    /// <summary>Số ngày thanh toán (khi PaymentCycle = AfterInvoice)</summary>
    public int? PaymentDays { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Department? Department { get; set; }
    public Account? UpdatedByAccount { get; set; }

    /// <summary>Năng lực: danh sách chỉ tiêu nhà thầu phụ có thể thực hiện</summary>
    public ICollection<SubcontractorCapability> SubcontractorCapabilities { get; set; } = new List<SubcontractorCapability>();
}
