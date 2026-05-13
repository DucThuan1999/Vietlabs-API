using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VietLab.Configuration;

namespace VietLab.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    private static void AddAddresses(MailAddressCollection collection, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return;
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            try
            {
                collection.Add(new MailAddress(part));
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Địa chỉ email không hợp lệ: {part}", ex);
            }
        }
    }

    public async Task SendQuotationPdfEmailAsync(
        string from,
        string to,
        string? cc,
        string? bcc,
        string subject,
        string body,
        IFormFile pdfFile,
        IReadOnlyList<IFormFile>? additionalAttachments = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
            throw new InvalidOperationException("SMTP chưa được cấu hình (Smtp:Host).");

        var fromAddr = string.IsNullOrWhiteSpace(from) ? _options.FromAddress : from.Trim();
        if (string.IsNullOrWhiteSpace(fromAddr))
            throw new InvalidOperationException("Thiếu địa chỉ From (và Smtp:FromAddress).");

        using var pdfStream = new MemoryStream();
        await pdfFile.CopyToAsync(pdfStream, cancellationToken);
        pdfStream.Position = 0;

        var safeName = string.IsNullOrWhiteSpace(pdfFile.FileName) ? "bao-gia.pdf" : Path.GetFileName(pdfFile.FileName);
        if (!safeName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            safeName += ".pdf";

        using var message = new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        var displayName = string.IsNullOrWhiteSpace(_options.FromName) ? null : _options.FromName;
        message.From = displayName == null
            ? new MailAddress(fromAddr)
            : new MailAddress(fromAddr, displayName);

        AddAddresses(message.To, to);
        AddAddresses(message.CC, cc);
        AddAddresses(message.Bcc, bcc);

        message.Attachments.Add(new Attachment(pdfStream, safeName, MediaTypeNames.Application.Pdf));

        if (additionalAttachments is { Count: > 0 })
        {
            foreach (var file in additionalAttachments)
            {
                if (file is not { Length: > 0 })
                    continue;

                var fileStream = new MemoryStream();
                await file.CopyToAsync(fileStream, cancellationToken);
                fileStream.Position = 0;

                var extraName = string.IsNullOrWhiteSpace(file.FileName)
                    ? "attachment"
                    : Path.GetFileName(file.FileName);
                if (string.IsNullOrWhiteSpace(extraName))
                    extraName = "attachment";

                message.Attachments.Add(new Attachment(fileStream, extraName));
            }
        }

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
        };

        if (!string.IsNullOrWhiteSpace(_options.UserName))
        {
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        }

        _logger.LogInformation("Sending quotation email via SMTP {Host}:{Port} to {To}", _options.Host, _options.Port, to);

        await client.SendMailAsync(message, cancellationToken);
    }
}
