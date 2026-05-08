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
public class OrderSamplePackagesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderSamplePackagesController> _logger;

    public OrderSamplePackagesController(ApplicationDbContext context, ILogger<OrderSamplePackagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderSamplePackages")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderSamplePackages
            .Include(p => p.OrderSample)
            .Include(p => p.SampleMatrix)
            .Include(p => p.OrderSamplePackageAnalysisItems)
            .ThenInclude(pai => pai.AnalysisItem));
    }

    [HttpGet("OrderSamplePackages({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var package = _context.OrderSamplePackages
            .Include(p => p.OrderSample)
            .Include(p => p.SampleMatrix)
            .Include(p => p.OrderSamplePackageAnalysisItems)
            .ThenInclude(pai => pai.AnalysisItem)
            .FirstOrDefault(p => p.OrderSamplePackageId == key);
        if (package == null)
        {
            return NotFound();
        }

        return Ok(package);
    }

    [HttpPost("OrderSamplePackages")]
    public async Task<IActionResult> Post([FromBody] OrderSamplePackage package)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        package.OrderSamplePackageId = package.OrderSamplePackageId == Guid.Empty
            ? Guid.NewGuid()
            : package.OrderSamplePackageId;
        package.CreatedAt = DateTime.UtcNow;
        _context.OrderSamplePackages.Add(package);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu gói mẫu đơn hàng");
        }

        return Created($"odata/OrderSamplePackages({package.OrderSamplePackageId})", package);
    }

    [HttpPut("OrderSamplePackages({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderSamplePackage package)
    {
        if (key != package.OrderSamplePackageId)
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật gói mẫu đơn hàng");
        }

        return Updated(package);
    }

    [HttpDelete("OrderSamplePackages({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var package = await _context.OrderSamplePackages.FindAsync(key);
        if (package == null)
        {
            return NotFound();
        }

        _context.OrderSamplePackages.Remove(package);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa gói mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderSamplePackages.Any(e => e.OrderSamplePackageId == key);
    }
}
