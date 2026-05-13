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
public class DesignationsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DesignationsController> _logger;

    public DesignationsController(ApplicationDbContext context, ILogger<DesignationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("Designations")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Designations.Include(d => d.UpdatedByAccount));
    }

    [HttpGet("Designations({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.Designations
            .Include(d => d.UpdatedByAccount)
            .FirstOrDefault(d => d.DesignationId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("Designations")]
    public async Task<IActionResult> Post([FromBody] Designation designation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        designation.DesignationId = designation.DesignationId == Guid.Empty ? Guid.NewGuid() : designation.DesignationId;
        designation.CreatedAt = DateTime.UtcNow;
        _context.Designations.Add(designation);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ định");
        }

        return Created($"odata/Designations({designation.DesignationId})", designation);
    }

    [HttpPut("Designations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Designation designation)
    {
        if (key != designation.DesignationId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.Designations.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SequenceNumber = designation.SequenceNumber;
        existing.DesignationCode = designation.DesignationCode;
        existing.SymbolCode = designation.SymbolCode;
        existing.Name = designation.Name;
        existing.Description = designation.Description;
        existing.Note = designation.Note;
        existing.Status = designation.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DesignationExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ định");
        }

        await _context.Entry(existing).Reference(d => d.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("Designations({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.Designations.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.Designations.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ định");
        }

        return NoContent();
    }

    private bool DesignationExists(Guid key)
    {
        return _context.Designations.Any(e => e.DesignationId == key);
    }
}
