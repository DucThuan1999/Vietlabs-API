using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VietLab.Configuration;

namespace VietLab.Services;

public class GraphEmailSender : IEmailSender
{
    private const string GraphScope = "https://graph.microsoft.com/.default";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient _httpClient;
    private readonly MicrosoftGraphMailOptions _options;
    private readonly ILogger<GraphEmailSender> _logger;
    private readonly TokenCredential _credential;

    public GraphEmailSender(
        HttpClient httpClient,
        IOptions<MicrosoftGraphMailOptions> options,
        ILogger<GraphEmailSender> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _credential = CreateCredential(_options);
    }

    private static TokenCredential CreateCredential(MicrosoftGraphMailOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.TenantId)
            || string.IsNullOrWhiteSpace(options.ClientId)
            || string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            return new UnconfiguredGraphCredential();
        }

        return new ClientSecretCredential(
            options.TenantId.Trim(),
            options.ClientId.Trim(),
            options.ClientSecret);
    }

    private static void AddRecipients(List<object> recipients, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return;

        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            try
            {
                _ = new System.Net.Mail.MailAddress(part);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException($"Địa chỉ email không hợp lệ: {part}", ex);
            }

            recipients.Add(new
            {
                emailAddress = new { address = part },
            });
        }
    }

    private static string NormalizePdfFileName(string? fileName)
    {
        var safeName = string.IsNullOrWhiteSpace(fileName) ? "bao-gia.pdf" : Path.GetFileName(fileName);
        if (!safeName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            safeName += ".pdf";
        return safeName;
    }

    private static string NormalizeAttachmentFileName(string? fileName)
    {
        var safeName = string.IsNullOrWhiteSpace(fileName)
            ? "attachment"
            : Path.GetFileName(fileName);
        return string.IsNullOrWhiteSpace(safeName) ? "attachment" : safeName;
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            _ => "application/octet-stream",
        };
    }

    private static async Task<(byte[] Content, string FileName, string ContentType)> ReadAttachmentAsync(
        IFormFile file,
        bool isPdf,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        var content = stream.ToArray();
        var fileName = isPdf
            ? NormalizePdfFileName(file.FileName)
            : NormalizeAttachmentFileName(file.FileName);
        var contentType = isPdf ? "application/pdf" : GetContentType(fileName);
        return (content, fileName, contentType);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.TenantId)
            || string.IsNullOrWhiteSpace(_options.ClientId)
            || string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new InvalidOperationException(
                "Microsoft Graph chưa được cấu hình (MicrosoftGraphMail:TenantId, ClientId, ClientSecret).");
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
        ValidateConfiguration();

        var fromAddress = string.IsNullOrWhiteSpace(from)
            ? _options.DefaultFromAddress.Trim()
            : from.Trim();
        if (string.IsNullOrWhiteSpace(fromAddress))
        {
            throw new InvalidOperationException(
                "Thiếu địa chỉ From (và MicrosoftGraphMail:DefaultFromAddress).");
        }

        try
        {
            _ = new System.Net.Mail.MailAddress(fromAddress);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Địa chỉ email From không hợp lệ: {fromAddress}", ex);
        }

        var toRecipients = new List<object>();
        AddRecipients(toRecipients, to);
        if (toRecipients.Count == 0)
            throw new ArgumentException("Thiếu người nhận To.");

        var ccRecipients = new List<object>();
        AddRecipients(ccRecipients, cc);

        var bccRecipients = new List<object>();
        AddRecipients(bccRecipients, bcc);

        var attachments = new List<object>();
        var totalAttachmentBytes = 0L;

        var pdfAttachment = await ReadAttachmentAsync(pdfFile, isPdf: true, cancellationToken);
        totalAttachmentBytes += pdfAttachment.Content.Length;
        attachments.Add(CreateFileAttachment(pdfAttachment.FileName, pdfAttachment.ContentType, pdfAttachment.Content));

        if (additionalAttachments is { Count: > 0 })
        {
            foreach (var file in additionalAttachments)
            {
                if (file is not { Length: > 0 })
                    continue;

                var extraAttachment = await ReadAttachmentAsync(file, isPdf: false, cancellationToken);
                totalAttachmentBytes += extraAttachment.Content.Length;
                attachments.Add(CreateFileAttachment(
                    extraAttachment.FileName,
                    extraAttachment.ContentType,
                    extraAttachment.Content));
            }
        }

        if (totalAttachmentBytes > _options.MaxInlineAttachmentBytes)
        {
            var maxMb = _options.MaxInlineAttachmentBytes / (1024d * 1024d);
            var actualMb = totalAttachmentBytes / (1024d * 1024d);
            throw new ArgumentException(
                $"Tổng dung lượng tệp đính kèm ({actualMb:0.##} MB) vượt giới hạn Graph sendMail ({maxMb:0.##} MB). Vui lòng giảm dung lượng file.");
        }

        var message = new Dictionary<string, object?>
        {
            ["subject"] = subject,
            ["body"] = new
            {
                contentType = "HTML",
                content = body,
            },
            ["toRecipients"] = toRecipients,
        };

        if (ccRecipients.Count > 0)
            message["ccRecipients"] = ccRecipients;
        if (bccRecipients.Count > 0)
            message["bccRecipients"] = bccRecipients;
        if (attachments.Count > 0)
            message["attachments"] = attachments;

        if (!string.IsNullOrWhiteSpace(_options.FromName))
        {
            message["from"] = new
            {
                emailAddress = new
                {
                    address = fromAddress,
                    name = _options.FromName,
                },
            };
        }

        var payload = new
        {
            message,
            saveToSentItems = true,
        };

        var requestUri = $"users/{Uri.EscapeDataString(fromAddress)}/sendMail";
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json"),
        };

        var accessToken = await _credential.GetTokenAsync(
            new TokenRequestContext([GraphScope]),
            cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);

        _logger.LogInformation(
            "Sending quotation email via Microsoft Graph as {From} to {To} ({AttachmentCount} attachments, {AttachmentBytes} bytes)",
            fromAddress,
            to,
            attachments.Count,
            totalAttachmentBytes);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return;

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var (graphCode, graphMessage) = TryParseGraphError(responseBody);
        var detail = graphMessage ?? response.ReasonPhrase ?? "Không thể gửi email qua Microsoft Graph.";
        if (!string.IsNullOrWhiteSpace(graphCode))
            detail = $"{detail} (Graph: {graphCode})";

        _logger.LogWarning(
            "Microsoft Graph sendMail failed for {From}. Status={StatusCode}, Code={GraphCode}, Body={Body}",
            fromAddress,
            (int)response.StatusCode,
            graphCode,
            responseBody);

        throw new GraphMailSendException(detail, (int)response.StatusCode, graphCode);
    }

    private static object CreateFileAttachment(string name, string contentType, byte[] content)
    {
        return new Dictionary<string, object?>
        {
            ["@odata.type"] = "#microsoft.graph.fileAttachment",
            ["name"] = name,
            ["contentType"] = contentType,
            ["contentBytes"] = Convert.ToBase64String(content),
        };
    }

    private static (string? Code, string? Message) TryParseGraphError(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return (null, null);

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (!document.RootElement.TryGetProperty("error", out var errorElement))
                return (null, null);

            string? code = errorElement.TryGetProperty("code", out var codeElement)
                ? codeElement.GetString()
                : null;
            string? message = errorElement.TryGetProperty("message", out var messageElement)
                ? messageElement.GetString()
                : null;
            return (code, message);
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }

    private sealed class UnconfiguredGraphCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            => throw new InvalidOperationException(
                "Microsoft Graph chưa được cấu hình (MicrosoftGraphMail:TenantId, ClientId, ClientSecret).");

        public override ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
            => throw new InvalidOperationException(
                "Microsoft Graph chưa được cấu hình (MicrosoftGraphMail:TenantId, ClientId, ClientSecret).");
    }
}
