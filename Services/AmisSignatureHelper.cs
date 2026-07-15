using System.Security.Cryptography;
using System.Text;

namespace VietLab.Services;

/// <summary>HMAC-SHA256 theo mẫu MISA ACT Open callback.</summary>
public static class AmisSignatureHelper
{
    public static string ComputeHmacSha256Hex(string input, string key)
    {
        input ??= string.Empty;
        var apiKey = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA256(apiKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        var hashString = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
            hashString.Append(b.ToString("X2").ToLowerInvariant());
        return hashString.ToString();
    }

    public static bool ValidateSignature(string? data, string? signature, string appId)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(appId))
            return false;

        var expected = ComputeHmacSha256Hex(data ?? string.Empty, appId);
        return string.Equals(expected, signature, StringComparison.Ordinal);
    }
}
