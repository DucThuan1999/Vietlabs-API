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
public class LaboratoryTechniquesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LaboratoryTechniquesController> _logger;

    public LaboratoryTechniquesController(ApplicationDbContext context, ILogger<LaboratoryTechniquesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("LaboratoryTechniques")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.LaboratoryTechniques.Include(lt => lt.UpdatedByAccount));
    }

    [HttpGet("LaboratoryTechniques({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.LaboratoryTechniques
            .Include(lt => lt.UpdatedByAccount)
            .FirstOrDefault(lt => lt.LaboratoryTechniqueId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("LaboratoryTechniques")]
    public async Task<IActionResult> Post([FromBody] LaboratoryTechnique entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        entity.LaboratoryTechniqueId = entity.LaboratoryTechniqueId == Guid.Empty ? Guid.NewGuid() : entity.LaboratoryTechniqueId;
        entity.CreatedAt = DateTime.UtcNow;
        _context.LaboratoryTechniques.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu kĩ thuật");
        }

        return Created($"odata/LaboratoryTechniques({entity.LaboratoryTechniqueId})", entity);
    }

    [HttpPut("LaboratoryTechniques({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] LaboratoryTechnique entity)
    {
        if (key != entity.LaboratoryTechniqueId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.LaboratoryTechniques.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SequenceNumber = entity.SequenceNumber;
        existing.TechniqueCode = entity.TechniqueCode;
        existing.NameVi = entity.NameVi;
        existing.NameEn = entity.NameEn;
        existing.Status = entity.Status;
        existing.Notes = entity.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LaboratoryTechniqueExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật kĩ thuật");
        }

        await _context.Entry(existing).Reference(lt => lt.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("LaboratoryTechniques({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.LaboratoryTechniques.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.LaboratoryTechniques.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa kĩ thuật");
        }

        return NoContent();
    }

    private bool LaboratoryTechniqueExists(Guid key)
    {
        return _context.LaboratoryTechniques.Any(e => e.LaboratoryTechniqueId == key);
    }
}
