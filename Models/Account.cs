namespace VietLab.Models;

public class Account
{
    public Guid AccountId { get; set; }
    public Guid EmployeeId { get; set; }         // Mỗi Account gắn 1 Employee
    public Guid PermissionId { get; set; }       // Mỗi Account gắn 1 Permission
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // Navigation
    public Employee? Employee { get; set; }
    public Permission? Permission { get; set; }
}


