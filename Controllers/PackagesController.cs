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
public class PackagesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PackagesController> _logger;

    public PackagesController(ApplicationDbContext context, ILogger<PackagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Cho phép $expand sâu tới AnalysisItem → ReferenceMethod/UnitOfMeasure/AnalysisItemTats (độ sâu &gt; 2).
    /// </summary>
    [HttpGet("Packages")]
    [EnableQuery(MaxExpansionDepth = 4)]
    public IActionResult Get()
    {
        // Keep the collection endpoint lightweight; OData $expand decides when related data is needed.
        return Ok(_context.Packages.AsNoTracking());
    }

    [HttpGet("Packages({key})")]
    [EnableQuery(MaxExpansionDepth = 4)]
    public IActionResult Get([FromRoute] Guid key)
    {
        var package = _context.Packages
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.PackageAnalysisItems)
                .ThenInclude(pai => pai.AnalysisItem!)
                    .ThenInclude(ai => ai.ReferenceMethod)
            .Include(p => p.PackageAnalysisItems)
                .ThenInclude(pai => pai.AnalysisItem!)
                    .ThenInclude(ai => ai.UnitOfMeasure)
            .Include(p => p.PackageAnalysisItems)
                .ThenInclude(pai => pai.AnalysisItem!)
                    .ThenInclude(ai => ai.AnalysisItemTats)
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu gói phân tích");
        }

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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật gói phân tích");
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa gói phân tích");
        }

        return NoContent();
    }

    private bool PackageExists(Guid key)
    {
        return _context.Packages.Any(e => e.PackageId == key);
    }
}

