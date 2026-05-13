namespace VietLab.Configuration;

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    /// <summary>Địa chỉ From mặc định khi client không gửi hoặc khi SMTP bắt buộc.</summary>
    public string FromAddress { get; set; } = "";
    public string? FromName { get; set; }
    public bool EnableSsl { get; set; } = true;
}
