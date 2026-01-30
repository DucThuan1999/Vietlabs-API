using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class AnalysisItemTatsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalysisItemTatsController> _logger;

    public AnalysisItemTatsController(
        ApplicationDbContext context,
        ILogger<AnalysisItemTatsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("AnalysisItemTats")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.AnalysisItemTats
            .Include(tat => tat.AnalysisItem));
    }

    [HttpGet("AnalysisItemTats({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var tat = _context.AnalysisItemTats
            .Include(tat => tat.AnalysisItem)
            .FirstOrDefault(tat => tat.AnalysisItemTatId == key);

        if (tat == null)
        {
            return NotFound();
        }

        return Ok(tat);
    }

    [HttpPost("AnalysisItemTats")]
    public async Task<IActionResult> Post([FromBody] AnalysisItemTat analysisItemTat)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate AnalysisItemId exists
        var analysisItemExists = await _context.AnalysisItems
            .AnyAsync(ai => ai.AnalysisItemId == analysisItemTat.AnalysisItemId);
        if (!analysisItemExists)
        {
            return BadRequest($"AnalysisItem with ID {analysisItemTat.AnalysisItemId} does not exist.");
        }

        // Validate TatType
        var validTatTypes = new[] { "Normal", "Fast", "Urgent", "Thường", "Nhanh", "Khẩn" };
        if (!validTatTypes.Contains(analysisItemTat.TatType))
        {
            return BadRequest($"TatType must be one of: {string.Join(", ", validTatTypes)}");
        }

        // Validate TatUnit
        var validTatUnits = new[] { "Days", "Hours", "Ngày", "Giờ" };
        if (!validTatUnits.Contains(analysisItemTat.TatUnit))
        {
            return BadRequest($"TatUnit must be one of: {string.Join(", ", validTatUnits)}");
        }

        // Validate TatValue > 0
        if (analysisItemTat.TatValue <= 0)
        {
            return BadRequest("TatValue must be greater than 0.");
        }

        // Check if TAT with same type already exists for this AnalysisItem
        var existingTat = await _context.AnalysisItemTats
            .FirstOrDefaultAsync(tat => tat.AnalysisItemId == analysisItemTat.AnalysisItemId 
                && tat.TatType == analysisItemTat.TatType);
        if (existingTat != null)
        {
            return BadRequest($"TAT with type '{analysisItemTat.TatType}' already exists for this AnalysisItem.");
        }

        // Set default values
        analysisItemTat.AnalysisItemTatId = analysisItemTat.AnalysisItemTatId == Guid.Empty 
            ? Guid.NewGuid() 
            : analysisItemTat.AnalysisItemTatId;
        if (analysisItemTat.CreatedAt == default)
        {
            analysisItemTat.CreatedAt = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(analysisItemTat.TatUnit))
        {
            analysisItemTat.TatUnit = "Days";
        }

        _context.AnalysisItemTats.Add(analysisItemTat);
        await _context.SaveChangesAsync();

        return Created($"odata/AnalysisItemTats({analysisItemTat.AnalysisItemTatId})", analysisItemTat);
    }

    [HttpPut("AnalysisItemTats({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] AnalysisItemTat analysisItemTat)
    {
        if (key != analysisItemTat.AnalysisItemTatId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate AnalysisItemId exists
        var analysisItemExists = await _context.AnalysisItems
            .AnyAsync(ai => ai.AnalysisItemId == analysisItemTat.AnalysisItemId);
        if (!analysisItemExists)
        {
            return BadRequest($"AnalysisItem with ID {analysisItemTat.AnalysisItemId} does not exist.");
        }

        // Validate TatType
        var validTatTypes = new[] { "Normal", "Fast", "Urgent", "Thường", "Nhanh", "Khẩn" };
        if (!validTatTypes.Contains(analysisItemTat.TatType))
        {
            return BadRequest($"TatType must be one of: {string.Join(", ", validTatTypes)}");
        }

        // Validate TatUnit
        var validTatUnits = new[] { "Days", "Hours", "Ngày", "Giờ" };
        if (!validTatUnits.Contains(analysisItemTat.TatUnit))
        {
            return BadRequest($"TatUnit must be one of: {string.Join(", ", validTatUnits)}");
        }

        // Validate TatValue > 0
        if (analysisItemTat.TatValue <= 0)
        {
            return BadRequest("TatValue must be greater than 0.");
        }

        // Check if another TAT with same type already exists for this AnalysisItem (excluding current)
        var existingTat = await _context.AnalysisItemTats
            .FirstOrDefaultAsync(tat => tat.AnalysisItemId == analysisItemTat.AnalysisItemId 
                && tat.TatType == analysisItemTat.TatType
                && tat.AnalysisItemTatId != key);
        if (existingTat != null)
        {
            return BadRequest($"TAT with type '{analysisItemTat.TatType}' already exists for this AnalysisItem.");
        }

        var existingAnalysisItemTat = await _context.AnalysisItemTats.FindAsync(key);
        if (existingAnalysisItemTat == null)
        {
            return NotFound();
        }

        _context.Entry(existingAnalysisItemTat).State = EntityState.Detached;
        analysisItemTat.UpdatedAt = DateTime.UtcNow;
        _context.Entry(analysisItemTat).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnalysisItemTatExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(analysisItemTat);
    }

    [HttpDelete("AnalysisItemTats({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var analysisItemTat = await _context.AnalysisItemTats.FindAsync(key);
        if (analysisItemTat == null)
        {
            return NotFound();
        }

        _context.AnalysisItemTats.Remove(analysisItemTat);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AnalysisItemTatExists(Guid key)
    {
        return _context.AnalysisItemTats.Any(e => e.AnalysisItemTatId == key);
    }
}

