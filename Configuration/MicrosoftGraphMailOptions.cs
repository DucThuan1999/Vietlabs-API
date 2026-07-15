namespace VietLab.Configuration;

/// <summary>
/// Cấu hình gửi email báo giá qua Microsoft Graph (app-only).
/// </summary>
public class MicrosoftGraphMailOptions
{
    public const string SectionName = "MicrosoftGraphMail";

    public string TenantId { get; set; } = "";

    public string ClientId { get; set; } = "";

    /// <summary>Client secret của Azure App Registration. Nên đặt qua biến môi trường hoặc user secrets.</summary>
    public string ClientSecret { get; set; } = "";

    /// <summary>Địa chỉ From mặc định khi client không gửi from.</summary>
    public string DefaultFromAddress { get; set; } = "";

    /// <summary>Tên hiển thị trên From (nếu mailbox hỗ trợ SendAs).</summary>
    public string? FromName { get; set; }

    /// <summary>Giới hạn tổng dung lượng attachment cho Graph sendMail trực tiếp (bytes).</summary>
    public long MaxInlineAttachmentBytes { get; set; } = 3 * 1024 * 1024;
}
