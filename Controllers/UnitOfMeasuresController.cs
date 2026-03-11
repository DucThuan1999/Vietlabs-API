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
public class UnitOfMeasuresController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UnitOfMeasuresController> _logger;

    public UnitOfMeasuresController(ApplicationDbContext context, ILogger<UnitOfMeasuresController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("UnitOfMeasures")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.UnitOfMeasures.Include(u => u.UpdatedByAccount));
    }

    [HttpGet("UnitOfMeasures({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.UnitOfMeasures
            .Include(u => u.UpdatedByAccount)
            .FirstOrDefault(u => u.UnitOfMeasureId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("UnitOfMeasures")]
    public async Task<IActionResult> Post([FromBody] UnitOfMeasure unitOfMeasure)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        unitOfMeasure.UnitOfMeasureId = unitOfMeasure.UnitOfMeasureId == Guid.Empty ? Guid.NewGuid() : unitOfMeasure.UnitOfMeasureId;
        unitOfMeasure.CreatedAt = DateTime.UtcNow;
        _context.UnitOfMeasures.Add(unitOfMeasure);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu đơn vị tính");
        }

        return Created($"odata/UnitOfMeasures({unitOfMeasure.UnitOfMeasureId})", unitOfMeasure);
    }

    [HttpPut("UnitOfMeasures({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] UnitOfMeasure unitOfMeasure)
    {
        if (key != unitOfMeasure.UnitOfMeasureId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.UnitOfMeasures.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SequenceNumber = unitOfMeasure.SequenceNumber;
        existing.UnitOfMeasureCode = unitOfMeasure.UnitOfMeasureCode;
        existing.NameVi = unitOfMeasure.NameVi;
        existing.NameEn = unitOfMeasure.NameEn;
        existing.Status = unitOfMeasure.Status;
        existing.Notes = unitOfMeasure.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UnitOfMeasureExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật đơn vị tính");
        }

        await _context.Entry(existing).Reference(u => u.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("UnitOfMeasures({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.UnitOfMeasures.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.UnitOfMeasures.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa đơn vị tính");
        }

        return NoContent();
    }

    private bool UnitOfMeasureExists(Guid key)
    {
        return _context.UnitOfMeasures.Any(e => e.UnitOfMeasureId == key);
    }
}
