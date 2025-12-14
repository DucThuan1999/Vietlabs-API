namespace VietLab.Models;

public class Permission
{
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty; // Code Quyền
    public string Name { get; set; } = string.Empty;           // Tên Quyền
    public string Notes { get; set; } = string.Empty;          // Mô tả/Ghi chú
    public string Status { get; set; } = "Active";             // Trạng thái

    // Navigation
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}


