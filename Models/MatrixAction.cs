namespace VietLab.Models;

/// <summary>Cột trong ma trận (View, Create, Edit, ...).</summary>
public class MatrixAction
{
    public Guid MatrixActionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameVi { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<SecurityModuleAction> ModuleActions { get; set; } = new List<SecurityModuleAction>();
    public ICollection<AccountModuleGrant> AccountGrants { get; set; } = new List<AccountModuleGrant>();
}
