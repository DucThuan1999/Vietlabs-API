namespace VietLab.Models;

public class Contact
{
    public Guid ContactId { get; set; }
    public Guid ClientId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Title { get; set; }
    public bool IsPrimary { get; set; }

    // Navigation
    public Client? Client { get; set; }
}


