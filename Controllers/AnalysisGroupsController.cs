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
public class AnalysisGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalysisGroupsController> _logger;

    public AnalysisGroupsController(ApplicationDbContext context, ILogger<AnalysisGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("AnalysisGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.AnalysisGroups.Include(ag => ag.AnalysisItems));
    }

    [HttpGet("AnalysisGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var group = _context.AnalysisGroups
            .Include(ag => ag.AnalysisItems)
            .FirstOrDefault(ag => ag.AnalysisGroupId == key);
        if (group == null)
        {
            return NotFound();
        }
        return Ok(group);
    }

    [HttpPost("AnalysisGroups")]
    public async Task<IActionResult> Post([FromBody] AnalysisGroup group)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        group.AnalysisGroupId = group.AnalysisGroupId == Guid.Empty ? Guid.NewGuid() : group.AnalysisGroupId;
        group.CreatedAt = DateTime.UtcNow;
        _context.AnalysisGroups.Add(group);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhóm phân tích");
        }

        return Created($"odata/AnalysisGroups({group.AnalysisGroupId})", group);
    }

    [HttpPut("AnalysisGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] AnalysisGroup group)
    {
        if (key != group.AnalysisGroupId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        group.UpdatedAt = DateTime.UtcNow;
        _context.Entry(group).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnalysisGroupExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhóm phân tích");
        }

        return Updated(group);
    }

    [HttpDelete("AnalysisGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var group = await _context.AnalysisGroups.FindAsync(key);
        if (group == null)
        {
            return NotFound();
        }

        _context.AnalysisGroups.Remove(group);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhóm phân tích");
        }

        return NoContent();
    }

    private bool AnalysisGroupExists(Guid key)
    {
        return _context.AnalysisGroups.Any(e => e.AnalysisGroupId == key);
    }
}

