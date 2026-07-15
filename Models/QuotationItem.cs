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

    // Phân biệt AnalysisItem standalone hay trong nhóm
    public bool? IsStandalone { get; set; } // true nếu AnalysisItem đứng riêng, false nếu trong nhóm, null nếu không phải AnalysisItem

    /// <summary>Vietlabs | Subcontractor. Null nếu không xác định.</summary>
    public string? CapacityType { get; set; }
    /// <summary>FK - Năng lực Vietlabs (khi CapacityType = Vietlabs).</summary>
    public Guid? DepartmentAnalysisCapabilityId { get; set; }
    /// <summary>FK - Năng lực nhà thầu phụ (khi CapacityType = Subcontractor).</summary>
    public Guid? SubcontractorCapabilityId { get; set; }

    // Thông tin hiển thị (có thể override từ master data)
    public string? ItemCode { get; set; } // Mã item
    public string? ItemNameVi { get; set; } // Tên tiếng Việt (text thuần, search)
    public string? ItemNameEn { get; set; } // Tên tiếng Anh (text thuần)
    public string? ItemDisplayNameVi { get; set; } // Tên hiển thị có format (JSON)
    public string? ItemDisplayNameEn { get; set; } // Tên EN hiển thị có format (JSON)
    public string? Description { get; set; } // Mô tả

    // Snapshot dữ liệu từ AnalysisItem (lưu khi tạo để backup)
    public string? SampleMatrixName { get; set; } // Tên nền mẫu (snapshot từ SampleMatrix.NameVi)
    public string? PublishedGroupCode { get; set; } // Mã phương pháp (snapshot từ AnalysisItem.PublishedGroupCode)
    public string? Unit { get; set; } // Đơn vị tính (snapshot từ AnalysisItem.UnitOfMeasure.NameVi)
    public string? Lod { get; set; } // Giới hạn phát hiện (snapshot từ AnalysisItem.Lod, format string)
    public string? Loq { get; set; } // Giới hạn định lượng (snapshot từ AnalysisItem.Loq, format string)
    public string? Tat { get; set; } // Thời gian quay vòng (snapshot từ AnalysisItemTat, format string)

    // Thông tin giá và số lượng
    public int Quantity { get; set; } = 1; // Số lượng
    public decimal? DefaultPrice { get; set; } // Đơn giá chuẩn (snapshot từ AnalysisItem.UnitPrice hoặc Package.DefaultPrice)
    public decimal UnitPrice { get; set; } // Đơn giá bán (có thể chỉnh sửa)
    public decimal? DiscountPercent { get; set; } // % giảm giá cho item này
    public decimal? DiscountAmount { get; set; } // Số tiền giảm giá
    public decimal SubTotal { get; set; } // Thành tiền (Quantity * UnitPrice - DiscountAmount)

    // Thông tin bổ sung
    public int? DisplayOrder { get; set; } // Thứ tự hiển thị
    public string? Notes { get; set; } // Ghi chú

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation Properties
    public Quotation? Quotation { get; set; }
    public AnalysisItem? AnalysisItem { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public Package? Package { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public DepartmentAnalysisCapability? DepartmentAnalysisCapability { get; set; }
    public SubcontractorCapability? SubcontractorCapability { get; set; }
}

