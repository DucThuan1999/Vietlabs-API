namespace VietLab.Models;

/// <summary>
/// Chi tiết báo giá - hỗ trợ 3 dạng: Chỉ tiêu, Nhóm chỉ tiêu, Gói
/// </summary>
public class QuotationItem
{
    public Guid QuotationItemId { get; set; }
    public Guid QuotationId { get; set; } // Foreign key đến Quotation

    // Loại item: "AnalysisItem", "AnalysisGroup", "Package"
    public string ItemType { get; set; } = string.Empty;

    // Foreign keys (chỉ một trong 3 có giá trị tùy theo ItemType)
    public Guid? AnalysisItemId { get; set; } // Nếu ItemType = "AnalysisItem"
    public Guid? AnalysisGroupId { get; set; } // Nếu ItemType = "AnalysisGroup"
    public Guid? PackageId { get; set; } // Nếu ItemType = "Package"

    // Thông tin hiển thị (có thể override từ master data)
    public string? ItemCode { get; set; } // Mã item
    public string? ItemNameVi { get; set; } // Tên tiếng Việt
    public string? ItemNameEn { get; set; } // Tên tiếng Anh
    public string? Description { get; set; } // Mô tả

    // Thông tin giá và số lượng
    public int Quantity { get; set; } = 1; // Số lượng
    public decimal UnitPrice { get; set; } // Đơn giá
    public decimal? DiscountPercent { get; set; } // % giảm giá cho item này
    public decimal? DiscountAmount { get; set; } // Số tiền giảm giá
    public decimal SubTotal { get; set; } // Thành tiền (Quantity * UnitPrice - DiscountAmount)

    // Thông tin bổ sung
    public int? DisplayOrder { get; set; } // Thứ tự hiển thị
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Quotation? Quotation { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Package? Package { get; set; }
}

