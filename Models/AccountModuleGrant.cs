namespace VietLab.Models;

/// <summary>Quyền đã gán cho tài khoản (một ô tick trong ma trận).</summary>
public class AccountModuleGrant
{
    public Guid AccountModuleGrantId { get; set; }
    public Guid AccountId { get; set; }
    public Guid SecurityModuleId { get; set; }
    public Guid MatrixActionId { get; set; }

    public Account? Account { get; set; }
    public SecurityModule? SecurityModule { get; set; }
    public MatrixAction? MatrixAction { get; set; }
}
