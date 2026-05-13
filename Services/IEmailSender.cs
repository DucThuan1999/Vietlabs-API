using Microsoft.AspNetCore.Http;

namespace VietLab.Services;

public interface IEmailSender
{
    Task SendQuotationPdfEmailAsync(
        string from,
        string to,
        string? cc,
        string? bcc,
        string subject,
        string body,
        IFormFile pdfFile,
        IReadOnlyList<IFormFile>? additionalAttachments = null,
        CancellationToken cancellationToken = default);
}
