using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class SampleMatrixGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public SampleMatrixGroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("SampleMatrixGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.SampleMatrixGroups.Include(smg => smg.SampleMatrices));
    }

    [HttpGet("SampleMatrixGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var group = _context.SampleMatrixGroups
            .Include(smg => smg.SampleMatrices)
            .FirstOrDefault(smg => smg.SampleMatrixGroupId == key);
        if (group == null)
        {
            return NotFound();
        }
        return Ok(group);
    }

    [HttpPost("SampleMatrixGroups")]
    public async Task<IActionResult> Post([FromBody] SampleMatrixGroup group)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        group.SampleMatrixGroupId = group.SampleMatrixGroupId == Guid.Empty ? Guid.NewGuid() : group.SampleMatrixGroupId;
        group.CreatedAt = DateTime.UtcNow;
        _context.SampleMatrixGroups.Add(group);
        await _context.SaveChangesAsync();

        return Created($"odata/SampleMatrixGroups({group.SampleMatrixGroupId})", group);
    }

    [HttpPut("SampleMatrixGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] SampleMatrixGroup group)
    {
        if (key != group.SampleMatrixGroupId)
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
            if (!SampleMatrixGroupExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(group);
    }

    [HttpDelete("SampleMatrixGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var group = await _context.SampleMatrixGroups.FindAsync(key);
        if (group == null)
        {
            return NotFound();
        }

        _context.SampleMatrixGroups.Remove(group);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SampleMatrixGroupExists(Guid key)
    {
        return _context.SampleMatrixGroups.Any(e => e.SampleMatrixGroupId == key);
    }
}

