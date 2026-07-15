using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace VietLab.Services;

/// <summary>
/// Ký + xác thực access token dạng "accountId:userName:timestamp" bằng HMAC-SHA256.
/// Thay cho token trước đây chỉ là Base64 thô của "accountId:userName:timestamp" — không ký nên
/// ai biết AccountId/UserName của người khác cũng tự tạo được token hợp lệ, và không có hạn dùng.
/// </summary>
public class AccessTokenService
{
    private readonly byte[] _signingKey;

    public TimeSpan AccessTokenLifetime { get; }

    public AccessTokenService(IConfiguration configuration)
    {
        var keyString = configuration["Auth:TokenSigningKey"];
        if (string.IsNullOrWhiteSpace(keyString))
        {
            throw new InvalidOperationException(
                "Auth:TokenSigningKey chưa được cấu hình. Đặt biến môi trường Auth__TokenSigningKey " +
                "(chuỗi base64 ngẫu nhiên, tối thiểu 32 byte) trước khi chạy ngoài môi trường Development.");
        }

        _signingKey = Convert.FromBase64String(keyString);

        var minutes = configuration.GetValue<int?>("Auth:AccessTokenLifetimeMinutes") ?? 60;
        AccessTokenLifetime = TimeSpan.FromMinutes(minutes);
    }

    public string Sign(Guid accountId, string userName)
    {
        var payload = $"{accountId}:{userName}:{DateTime.UtcNow:yyyyMMddHHmmss}";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA256(_signingKey);
        var signature = hmac.ComputeHash(payloadBytes);
        return $"{Convert.ToBase64String(payloadBytes)}.{Convert.ToBase64String(signature)}";
    }

    /// <summary>Xác thực chữ ký + hạn dùng. Trả false nếu token bị sửa, sai định dạng, hoặc hết hạn.</summary>
    public bool TryValidate(string? token, out Guid accountId, out string userName)
    {
        accountId = Guid.Empty;
        userName = string.Empty;
        if (string.IsNullOrWhiteSpace(token)) return false;

        var parts = token.Split('.');
        if (parts.Length != 2) return false;

        byte[] payloadBytes;
        byte[] signatureBytes;
        try
        {
            payloadBytes = Convert.FromBase64String(parts[0]);
            signatureBytes = Convert.FromBase64String(parts[1]);
        }
        catch (FormatException)
        {
            return false;
        }

        using (var hmac = new HMACSHA256(_signingKey))
        {
            var expectedSignature = hmac.ComputeHash(payloadBytes);
            if (!CryptographicOperations.FixedTimeEquals(signatureBytes, expectedSignature))
            {
                return false;
            }
        }

        var payload = Encoding.UTF8.GetString(payloadBytes);
        var tokenParts = payload.Split(':');
        if (tokenParts.Length < 3) return false;
        if (!Guid.TryParse(tokenParts[0], out accountId)) return false;
        userName = tokenParts[1];

        if (!DateTime.TryParseExact(
                tokenParts[2],
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var issuedAt))
        {
            return false;
        }

        return DateTime.UtcNow - issuedAt <= AccessTokenLifetime;
    }
}
