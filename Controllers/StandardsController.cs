using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Helpers;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class StandardsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StandardsController> _logger;

    public StandardsController(ApplicationDbContext context, ILogger<StandardsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("Standards")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Standards.Include(s => s.UpdatedByAccount));
    }

    [HttpGet("Standards({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.Standards
            .Include(s => s.UpdatedByAccount)
            .FirstOrDefault(s => s.StandardId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("Standards")]
    public async Task<IActionResult> Post([FromBody] Standard standard)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        standard.StandardId = standard.StandardId == Guid.Empty ? Guid.NewGuid() : standard.StandardId;
        standard.CreatedAt = DateTime.UtcNow;
        _context.Standards.Add(standard);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu tiêu chuẩn/qui chuẩn");
        }

        return Created($"odata/Standards({standard.StandardId})", standard);
    }

    [HttpPut("Standards({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Standard standard)
    {
        if (key != standard.StandardId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.Standards.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SequenceNumber = standard.SequenceNumber;
        existing.StandardCode = standard.StandardCode;
        existing.NameVi = standard.NameVi;
        existing.NameEn = standard.NameEn;
        existing.Status = standard.Status;
        existing.Notes = standard.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StandardExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật tiêu chuẩn/qui chuẩn");
        }

        await _context.Entry(existing).Reference(s => s.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("Standards({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.Standards.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.Standards.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa tiêu chuẩn/qui chuẩn");
        }

        return NoContent();
    }

    private bool StandardExists(Guid key)
    {
        return _context.Standards.Any(e => e.StandardId == key);
    }
}
