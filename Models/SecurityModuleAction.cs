namespace VietLab.Models;

/// <summary>Ô khả dụng trong ma trận (module × hành động).</summary>
public class SecurityModuleAction
{
    public Guid SecurityModuleId { get; set; }
    public Guid MatrixActionId { get; set; }

    public SecurityModule? SecurityModule { get; set; }
    public MatrixAction? MatrixAction { get; set; }
}
