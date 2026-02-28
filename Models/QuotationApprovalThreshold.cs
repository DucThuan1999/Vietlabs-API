namespace VietLab.Models;

/// <summary>
/// Bảng cấu hình phần trăm phê duyệt báo giá
/// </summary>
public class QuotationApprovalThreshold
{
    public Guid QuotationApprovalThresholdId { get; set; }
    
    /// <summary>
    /// Phần trăm giảm giá tối thiểu (Min %)
    /// </summary>
    public decimal MinDiscountPercent { get; set; }
    
    /// <summary>
    /// Phần trăm giảm giá tối đa (Max %)
    /// </summary>
    public decimal MaxDiscountPercent { get; set; }
    
    /// <summary>
    /// Số cấp phê duyệt cần thiết (0 = không cần, 1 = 1 cấp, 2 = 2 cấp)
    /// </summary>
    public int ApprovalLevels { get; set; }
    
    /// <summary>
    /// Mô tả
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Trạng thái (Active/Inactive)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}

