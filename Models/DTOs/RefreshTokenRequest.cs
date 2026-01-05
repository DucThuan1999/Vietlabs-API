namespace VietLab.Models.DTOs;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; } // Access token mới
    public string? RefreshToken { get; set; } // Refresh token mới (nếu rotate)
    public DateTime? TokenExpiresAt { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
}

