using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class PackagesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public PackagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Packages")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Packages
            .Include(p => p.PackageAnalysisGroups)
                .ThenInclude(pag => pag.AnalysisGroup)
            .Include(p => p.SampleMatrix));
    }

    [HttpGet("Packages({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var package = _context.Packages
            .Include(p => p.PackageAnalysisGroups)
                .ThenInclude(pag => pag.AnalysisGroup)
            .Include(p => p.SampleMatrix)
            .FirstOrDefault(p => p.PackageId == key);
        if (package == null)
        {
            return NotFound();
        }
        return Ok(package);
    }

    [HttpPost("Packages")]
    public async Task<IActionResult> Post([FromBody] Package package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        package.PackageId = package.PackageId == Guid.Empty ? Guid.NewGuid() : package.PackageId;
        package.CreatedAt = DateTime.UtcNow;
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        return Created($"odata/Packages({package.PackageId})", package);
    }

    [HttpPut("Packages({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Package package)
    {
        if (key != package.PackageId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        package.UpdatedAt = DateTime.UtcNow;
        _context.Entry(package).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PackageExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(package);
    }

    [HttpDelete("Packages({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var package = await _context.Packages.FindAsync(key);
        if (package == null)
        {
            return NotFound();
        }

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PackageExists(Guid key)
    {
        return _context.Packages.Any(e => e.PackageId == key);
    }
}

