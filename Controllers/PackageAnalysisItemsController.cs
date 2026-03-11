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
public class PackageAnalysisItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PackageAnalysisItemsController> _logger;

    public PackageAnalysisItemsController(ApplicationDbContext context, ILogger<PackageAnalysisItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("PackageAnalysisItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.PackageAnalysisItems
            .Include(pai => pai.Package)
            .Include(pai => pai.AnalysisItem));
    }

    [HttpGet("PackageAnalysisItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var packageAnalysisItem = _context.PackageAnalysisItems
            .Include(pai => pai.Package)
            .Include(pai => pai.AnalysisItem)
            .FirstOrDefault(pai => pai.PackageAnalysisItemId == key);
        if (packageAnalysisItem == null)
        {
            return NotFound();
        }
        return Ok(packageAnalysisItem);
    }

    [HttpPost("PackageAnalysisItems")]
    public async Task<IActionResult> Post([FromBody] PackageAnalysisItem packageAnalysisItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        packageAnalysisItem.PackageAnalysisItemId = packageAnalysisItem.PackageAnalysisItemId == Guid.Empty
            ? Guid.NewGuid()
            : packageAnalysisItem.PackageAnalysisItemId;
        packageAnalysisItem.CreatedAt = DateTime.UtcNow;
        _context.PackageAnalysisItems.Add(packageAnalysisItem);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu gói");
        }

        return Created($"odata/PackageAnalysisItems({packageAnalysisItem.PackageAnalysisItemId})", packageAnalysisItem);
    }

    [HttpPut("PackageAnalysisItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] PackageAnalysisItem packageAnalysisItem)
    {
        if (key != packageAnalysisItem.PackageAnalysisItemId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(packageAnalysisItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PackageAnalysisItemExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu gói");
        }

        return Updated(packageAnalysisItem);
    }

    [HttpDelete("PackageAnalysisItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var packageAnalysisItem = await _context.PackageAnalysisItems.FindAsync(key);
        if (packageAnalysisItem == null)
        {
            return NotFound();
        }

        _context.PackageAnalysisItems.Remove(packageAnalysisItem);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu gói");
        }

        return NoContent();
    }

    private bool PackageAnalysisItemExists(Guid key)
    {
        return _context.PackageAnalysisItems.Any(e => e.PackageAnalysisItemId == key);
    }
}
