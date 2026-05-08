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
public class OrderTemplatePackagesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderTemplatePackagesController> _logger;

    public OrderTemplatePackagesController(ApplicationDbContext context, ILogger<OrderTemplatePackagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderTemplatePackages")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderTemplatePackages
            .Include(p => p.OrderTemplate)
            .Include(p => p.SampleMatrix)
            .Include(p => p.OrderTemplatePackageAnalysisItems)
            .ThenInclude(pai => pai.AnalysisItem));
    }

    [HttpGet("OrderTemplatePackages({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var package = _context.OrderTemplatePackages
            .Include(p => p.OrderTemplate)
            .Include(p => p.SampleMatrix)
            .Include(p => p.OrderTemplatePackageAnalysisItems)
            .ThenInclude(pai => pai.AnalysisItem)
            .FirstOrDefault(p => p.OrderTemplatePackageId == key);
        if (package == null)
        {
            return NotFound();
        }

        return Ok(package);
    }

    [HttpPost("OrderTemplatePackages")]
    public async Task<IActionResult> Post([FromBody] OrderTemplatePackage package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        package.OrderTemplatePackageId = package.OrderTemplatePackageId == Guid.Empty
            ? Guid.NewGuid()
            : package.OrderTemplatePackageId;
        package.CreatedAt = DateTime.UtcNow;
        _context.OrderTemplatePackages.Add(package);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu gói template mẫu đơn hàng");
        }

        return Created($"odata/OrderTemplatePackages({package.OrderTemplatePackageId})", package);
    }

    [HttpPut("OrderTemplatePackages({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderTemplatePackage package)
    {
        if (key != package.OrderTemplatePackageId)
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
            if (!Exists(key))
            {
                return NotFound();
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật gói template mẫu đơn hàng");
        }

        return Updated(package);
    }

    [HttpDelete("OrderTemplatePackages({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var package = await _context.OrderTemplatePackages.FindAsync(key);
        if (package == null)
        {
            return NotFound();
        }

        _context.OrderTemplatePackages.Remove(package);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa gói template mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderTemplatePackages.Any(e => e.OrderTemplatePackageId == key);
    }
}
