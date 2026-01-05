namespace VietLab.Models;

public class RefreshToken
{
    public Guid RefreshTokenId { get; set; }
    public Guid AccountId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
    public string? RevokedReason { get; set; }

    // Navigation
    public Account? Account { get; set; }
}

