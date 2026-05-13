using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;
using VietLab.Services;

namespace VietLab.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationEmailsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<QuotationEmailsController> _logger;

    public QuotationEmailsController(
        ApplicationDbContext context,
        IEmailSender emailSender,
        ILogger<QuotationEmailsController> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        if (Guid.TryParse(accountIdClaim, out var accountId))
            return accountId;
        return null;
    }

    private const int MaxExtraEmailAttachments = 15;

    /// <summary>Gửi email báo giá kèm PDF báo giá và (tùy chọn) tệp đính kèm thêm. Chỉ báo giá đã Active/Expired.</summary>
    [HttpPost("send")]
    [RequestSizeLimit(52_428_800)] // ~50 MB
    public async Task<IActionResult> Send(
        [FromForm] Guid quotationId,
        [FromForm] string from,
        [FromForm] string to,
        [FromForm] string? cc,
        [FromForm] string? bcc,
        [FromForm] string subject,
        [FromForm] string body,
        IFormFile pdfFile,
        [FromForm] List<IFormFile>? extraAttachments,
        CancellationToken cancellationToken)
    {
        if (pdfFile == null || pdfFile.Length == 0)
            return BadRequest(new { title = "Thiếu file PDF", detail = "Vui lòng đính kèm file PDF." });

        if (string.IsNullOrWhiteSpace(to))
            return BadRequest(new { title = "Thiếu người nhận", detail = "To là bắt buộc." });

        var extras = (extraAttachments ?? [])
            .Where(f => f is { Length: > 0 })
            .ToList();
        if (extras is { Count: > MaxExtraEmailAttachments })
            return BadRequest(new
            {
                title = "Quá nhiều tệp đính kèm",
                detail = $"Tối đa {MaxExtraEmailAttachments} tệp đính kèm thêm (ngoài PDF báo giá).",
            });

        var quotation = await _context.Quotations.AsNoTracking()
            .FirstOrDefaultAsync(q => q.QuotationId == quotationId, cancellationToken);

        if (quotation == null)
            return NotFound(new { title = "Không tìm thấy báo giá", detail = quotationId.ToString() });

        var st = quotation.Status ?? "";
        var emailAllowed = string.Equals(st, "Active", StringComparison.Ordinal)
            || string.Equals(st, "Expired", StringComparison.Ordinal);
        if (!emailAllowed)
            return StatusCode(StatusCodes.Status403Forbidden,
                new { title = "Không được gửi email", detail = "Chỉ báo giá đang hiệu lực hoặc đã hết hiệu lực mới được gửi email kèm PDF." });

        try
        {
            await _emailSender.SendQuotationPdfEmailAsync(
                from,
                to,
                cc,
                bcc,
                subject,
                body,
                pdfFile,
                extras,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Send quotation email configuration error");
            return BadRequest(new { title = "Cấu hình email", detail = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { title = "Dữ liệu không hợp lệ", detail = ex.Message });
        }

        var accountId = GetCurrentAccountId();
        if (accountId.HasValue && await _context.Accounts.AnyAsync(a => a.AccountId == accountId.Value, cancellationToken))
        {
            var history = new QuotationHistory
            {
                QuotationHistoryId = Guid.NewGuid(),
                QuotationId = quotationId,
                ChangedDate = DateTime.UtcNow,
                ChangeType = "EmailSent",
                ChangeDescription = $"Gửi email báo giá tới {to}. Tiêu đề: {subject}",
                ChangedByAccountId = accountId.Value,
                NewValues = JsonSerializer.Serialize(new { from, to, cc, bcc, subject }),
            };
            _context.QuotationHistories.Add(history);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _logger.LogInformation("Quotation email sent but history skipped (no AccountId on token).");
        }

        return Ok(new { success = true });
    }
}
