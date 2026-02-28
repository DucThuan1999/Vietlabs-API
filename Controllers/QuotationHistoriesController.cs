using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VietLab.Data;
using VietLab.Helpers;
using VietLab.Models;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class QuotationHistoriesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationHistoriesController> _logger;

    public QuotationHistoriesController(
        ApplicationDbContext context,
        ILogger<QuotationHistoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        if (Guid.TryParse(accountIdClaim, out var accountId))
        {
            return accountId;
        }
        return null;
    }

    [HttpGet("QuotationHistories")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationHistories
            .Include(qh => qh.Quotation)
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee));
    }

    [HttpGet("QuotationHistories({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var history = _context.QuotationHistories
            .Include(qh => qh.Quotation)
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefault(qh => qh.QuotationHistoryId == key);

        if (history == null)
        {
            return NotFound();
        }

        return Ok(history);
    }

    [HttpGet("Quotations({quotationId})/QuotationHistories")]
    [EnableQuery]
    public IActionResult GetByQuotation([FromRoute] Guid quotationId)
    {
        var histories = _context.QuotationHistories
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .Where(qh => qh.QuotationId == quotationId)
            .OrderByDescending(qh => qh.ChangedDate);

        return Ok(histories);
    }

    [HttpPost("QuotationHistories")]
    public async Task<IActionResult> Post([FromBody] QuotationHistory quotationHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate QuotationId exists
        var quotationExists = await _context.Quotations.AnyAsync(q => q.QuotationId == quotationHistory.QuotationId);
        if (!quotationExists)
        {
            return BadRequest($"Quotation with ID {quotationHistory.QuotationId} does not exist.");
        }

        // Validate ChangedByAccountId exists
        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == quotationHistory.ChangedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {quotationHistory.ChangedByAccountId} does not exist.");
        }

        // Set default values
        quotationHistory.QuotationHistoryId = quotationHistory.QuotationHistoryId == Guid.Empty ? Guid.NewGuid() : quotationHistory.QuotationHistoryId;
        if (quotationHistory.ChangedDate == default)
        {
            quotationHistory.ChangedDate = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(quotationHistory.ChangeType))
        {
            quotationHistory.ChangeType = "Manual";
        }

        _context.QuotationHistories.Add(quotationHistory);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu lịch sử báo giá");
        }

        // Reload với đầy đủ navigation properties
        var createdHistory = await _context.QuotationHistories
            .Include(qh => qh.Quotation)
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(qh => qh.QuotationHistoryId == quotationHistory.QuotationHistoryId);

        return Created($"odata/QuotationHistories({quotationHistory.QuotationHistoryId})", createdHistory);
    }

    [HttpPut("QuotationHistories({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationHistory quotationHistory)
    {
        if (key != quotationHistory.QuotationHistoryId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate QuotationId exists
        var quotationExists = await _context.Quotations.AnyAsync(q => q.QuotationId == quotationHistory.QuotationId);
        if (!quotationExists)
        {
            return BadRequest($"Quotation with ID {quotationHistory.QuotationId} does not exist.");
        }

        // Validate ChangedByAccountId exists
        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == quotationHistory.ChangedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {quotationHistory.ChangedByAccountId} does not exist.");
        }

        var existingHistory = await _context.QuotationHistories.FindAsync(key);
        if (existingHistory == null)
        {
            return NotFound();
        }

        _context.Entry(existingHistory).State = EntityState.Detached;
        _context.Entry(quotationHistory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationHistoryExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật lịch sử báo giá");
        }

        // Reload với đầy đủ navigation properties
        var updatedHistory = await _context.QuotationHistories
            .Include(qh => qh.Quotation)
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(qh => qh.QuotationHistoryId == key);

        return Updated(updatedHistory);
    }

    [HttpDelete("QuotationHistories({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var quotationHistory = await _context.QuotationHistories.FindAsync(key);
        if (quotationHistory == null)
        {
            return NotFound();
        }

        _context.QuotationHistories.Remove(quotationHistory);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa lịch sử báo giá");
        }

        return NoContent();
    }

    private bool QuotationHistoryExists(Guid key)
    {
        return _context.QuotationHistories.Any(e => e.QuotationHistoryId == key);
    }
}

