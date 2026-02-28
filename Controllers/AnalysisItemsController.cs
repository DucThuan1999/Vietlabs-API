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
public class AnalysisItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalysisItemsController> _logger;

    public AnalysisItemsController(ApplicationDbContext context, ILogger<AnalysisItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("AnalysisItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.AnalysisItems
            .Include(ai => ai.EquipmentType)
            .Include(ai => ai.AnalysisGroup)
            .Include(ai => ai.SampleMatrix)
            .Include(ai => ai.SampleMatrixGroup)
            .Include(ai => ai.AnalysisItemTats));
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
            .Include(ai => ai.AnalysisItemTats)
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu phân tích");
        }

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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu phân tích");
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu phân tích");
        }

        return NoContent();
    }

    private bool AnalysisItemExists(Guid key)
    {
        return _context.AnalysisItems.Any(e => e.AnalysisItemId == key);
    }
}

