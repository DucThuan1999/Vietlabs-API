namespace VietLab.Models;

public class Permission
{
    public Guid PermissionId { get; set; }
    public string? PermissionCode { get; set; } // Code Quyền
    public string Name { get; set; } = string.Empty;           // Tên Quyền
    public string? Notes { get; set; }          // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";             // Trạng thái

    // Navigation
    public ICollection<ModuleApprover> ModuleApprovers { get; set; } = new List<ModuleApprover>();
}
