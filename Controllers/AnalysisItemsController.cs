using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class AnalysisItemsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public AnalysisItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("AnalysisItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.AnalysisItems
            .Include(ai => ai.EquipmentType)
            .Include(ai => ai.AnalysisGroup)
            .Include(ai => ai.SampleMatrix)
            .Include(ai => ai.SampleMatrixGroup));
    }

    [HttpGet("AnalysisItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.AnalysisItems
            .Include(ai => ai.EquipmentType)
            .Include(ai => ai.AnalysisGroup)
            .Include(ai => ai.SampleMatrix)
            .Include(ai => ai.SampleMatrixGroup)
            .FirstOrDefault(ai => ai.AnalysisItemId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("AnalysisItems")]
    public async Task<IActionResult> Post([FromBody] AnalysisItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.AnalysisItemId = item.AnalysisItemId == Guid.Empty ? Guid.NewGuid() : item.AnalysisItemId;
        item.CreatedAt = DateTime.UtcNow;
        _context.AnalysisItems.Add(item);
        await _context.SaveChangesAsync();

        return Created($"odata/AnalysisItems({item.AnalysisItemId})", item);
    }

    [HttpPut("AnalysisItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] AnalysisItem item)
    {
        if (key != item.AnalysisItemId)
        {
            return BadRequest();
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
            if (!AnalysisItemExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(item);
    }

    [HttpDelete("AnalysisItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.AnalysisItems.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.AnalysisItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AnalysisItemExists(Guid key)
    {
        return _context.AnalysisItems.Any(e => e.AnalysisItemId == key);
    }
}

