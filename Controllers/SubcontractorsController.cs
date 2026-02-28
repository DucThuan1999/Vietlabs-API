using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Helpers;
using VietLab.Models;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class SubcontractorsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubcontractorsController> _logger;

    public SubcontractorsController(
        ApplicationDbContext context,
        ILogger<SubcontractorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Subcontractors")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Subcontractors.Include(s => s.Department));
    }

    [HttpGet("Subcontractors({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.Subcontractors.Include(s => s.Department).FirstOrDefault(c => c.SubcontractorId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("Subcontractors")]
    public async Task<IActionResult> Post([FromBody] Subcontractor item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.SubcontractorId = item.SubcontractorId == Guid.Empty ? Guid.NewGuid() : item.SubcontractorId;
        if (string.IsNullOrEmpty(item.Status))
        {
            item.Status = "Active";
        }
        item.CreatedAt = DateTime.UtcNow;

        _context.Subcontractors.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhà thầu phụ");
        }

        return Created($"odata/Subcontractors({item.SubcontractorId})", item);
    }

    [HttpPut("Subcontractors({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Subcontractor item)
    {
        if (key != item.SubcontractorId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.UpdatedAt = DateTime.UtcNow;
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubcontractorExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhà thầu phụ");
        }

        return Updated(item);
    }

    [HttpPatch("Subcontractors({key})")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] Delta<Subcontractor> patch)
    {
        var item = await _context.Subcontractors.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        patch.Patch(item);
        item.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhà thầu phụ");
        }

        return Updated(item);
    }

    [HttpDelete("Subcontractors({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.Subcontractors.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.Subcontractors.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhà thầu phụ");
        }

        return NoContent();
    }

    private bool SubcontractorExists(Guid key)
    {
        return _context.Subcontractors.Any(e => e.SubcontractorId == key);
    }
}
