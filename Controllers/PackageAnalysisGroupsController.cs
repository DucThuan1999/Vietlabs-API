using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class PackageAnalysisGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public PackageAnalysisGroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("PackageAnalysisGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.PackageAnalysisGroups
            .Include(pag => pag.Package)
            .Include(pag => pag.AnalysisGroup));
    }

    [HttpGet("PackageAnalysisGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var packageAnalysisGroup = _context.PackageAnalysisGroups
            .Include(pag => pag.Package)
            .Include(pag => pag.AnalysisGroup)
            .FirstOrDefault(pag => pag.PackageAnalysisGroupId == key);
        if (packageAnalysisGroup == null)
        {
            return NotFound();
        }
        return Ok(packageAnalysisGroup);
    }

    [HttpPost("PackageAnalysisGroups")]
    public async Task<IActionResult> Post([FromBody] PackageAnalysisGroup packageAnalysisGroup)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        packageAnalysisGroup.PackageAnalysisGroupId = packageAnalysisGroup.PackageAnalysisGroupId == Guid.Empty 
            ? Guid.NewGuid() 
            : packageAnalysisGroup.PackageAnalysisGroupId;
        packageAnalysisGroup.CreatedAt = DateTime.UtcNow;
        _context.PackageAnalysisGroups.Add(packageAnalysisGroup);
        await _context.SaveChangesAsync();

        return Created($"odata/PackageAnalysisGroups({packageAnalysisGroup.PackageAnalysisGroupId})", packageAnalysisGroup);
    }

    [HttpPut("PackageAnalysisGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] PackageAnalysisGroup packageAnalysisGroup)
    {
        if (key != packageAnalysisGroup.PackageAnalysisGroupId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(packageAnalysisGroup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PackageAnalysisGroupExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(packageAnalysisGroup);
    }

    [HttpDelete("PackageAnalysisGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var packageAnalysisGroup = await _context.PackageAnalysisGroups.FindAsync(key);
        if (packageAnalysisGroup == null)
        {
            return NotFound();
        }

        _context.PackageAnalysisGroups.Remove(packageAnalysisGroup);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PackageAnalysisGroupExists(Guid key)
    {
        return _context.PackageAnalysisGroups.Any(e => e.PackageAnalysisGroupId == key);
    }
}

