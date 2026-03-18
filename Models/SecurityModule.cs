namespace VietLab.Models;

/// <summary>Hàng trong ma trận quyền (Admin, Báo giá, ...).</summary>
public class SecurityModule
{
    public Guid SecurityModuleId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameVi { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string Status { get; set; } = "Active";

    public ICollection<SecurityModuleAction> ModuleActions { get; set; } = new List<SecurityModuleAction>();
    public ICollection<AccountModuleGrant> AccountGrants { get; set; } = new List<AccountModuleGrant>();
}
