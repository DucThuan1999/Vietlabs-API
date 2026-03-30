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
public class SectionsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SectionsController> _logger;

    public SectionsController(ApplicationDbContext context, ILogger<SectionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("Sections")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Sections.Include(s => s.UpdatedByAccount));
    }

    [HttpGet("Sections({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var section = _context.Sections.Include(s => s.UpdatedByAccount).FirstOrDefault(s => s.SectionId == key);
        if (section == null)
        {
            return NotFound();
        }
        return Ok(section);
    }

    [HttpPost("Sections")]
    public async Task<IActionResult> Post([FromBody] Section section)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        section.SectionId = section.SectionId == Guid.Empty ? Guid.NewGuid() : section.SectionId;
        _context.Sections.Add(section);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu bộ phận");
        }

        return Created($"odata/Sections({section.SectionId})", section);
    }

    [HttpPut("Sections({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Section section)
    {
        if (key != section.SectionId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.Sections.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SectionCode = section.SectionCode;
        existing.DepartmentId = section.DepartmentId;
        existing.NameVi = section.NameVi;
        existing.NameEn = section.NameEn;
        existing.Notes = section.Notes;
        existing.Status = section.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SectionExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật bộ phận");
        }

        var updated = await _context.Sections.Include(s => s.UpdatedByAccount).FirstOrDefaultAsync(s => s.SectionId == key);
        return Updated(updated ?? existing);
    }

    [HttpDelete("Sections({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var section = await _context.Sections.FindAsync(key);
        if (section == null)
        {
            return NotFound();
        }

        _context.Sections.Remove(section);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa bộ phận");
        }

        return NoContent();
    }

    private bool SectionExists(Guid key)
    {
        return _context.Sections.Any(s => s.SectionId == key);
    }
}
