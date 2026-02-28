namespace VietLab.Models;

/// <summary>
/// Bảng cấu hình người phê duyệt theo module
/// Hỗ trợ phân quyền theo User chỉ định hoặc theo Title (chức vụ)
/// </summary>
public class ModuleApprover
{
    public Guid ModuleApproverId { get; set; }
    
    /// <summary>
    /// Tên module (Quotation, Client, Package, StoreRecord, etc.)
    /// </summary>
    public string ModuleCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Cấp phê duyệt (1, 2, 3...)
    /// Cấp 1 thường là manager của employee (tự động từ Employee.ManagerId)
    /// </summary>
    public int ApprovalLevel { get; set; }
    
    /// <summary>
    /// Loại người phê duyệt: "User" (người chỉ định) hoặc "Title" (theo chức vụ)
    /// </summary>
    public string ApproverType { get; set; } = "User"; // User hoặc Title
    
    /// <summary>
    /// ID Employee phê duyệt (nếu ApproverType = "User")
    /// </summary>
    public Guid? ApproverEmployeeId { get; set; }
    
    /// <summary>
    /// Chức vụ phê duyệt (nếu ApproverType = "Title")
    /// Ví dụ: "Giám đốc", "Phó giám đốc", "Trưởng phòng"
    /// </summary>
    public string? ApproverTitle { get; set; }
    
    /// <summary>
    /// Liên kết với Permission (optional)
    /// Nếu null, áp dụng cho toàn hệ thống
    /// Nếu có giá trị, chỉ áp dụng cho Permission đó
    /// </summary>
    public Guid? PermissionId { get; set; }
    
    /// <summary>
    /// Mô tả/Ghi chú
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Trạng thái (Active/Inactive)
    /// </summary>
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation Properties
    public Employee? ApproverEmployee { get; set; }
    public Permission? Permission { get; set; }
}

