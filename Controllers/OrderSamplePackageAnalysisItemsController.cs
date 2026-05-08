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
public class OrderSamplePackageAnalysisItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderSamplePackageAnalysisItemsController> _logger;

    public OrderSamplePackageAnalysisItemsController(ApplicationDbContext context, ILogger<OrderSamplePackageAnalysisItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderSamplePackageAnalysisItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderSamplePackageAnalysisItems
            .Include(pai => pai.OrderSamplePackage)
            .Include(pai => pai.AnalysisItem));
    }

    [HttpGet("OrderSamplePackageAnalysisItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.OrderSamplePackageAnalysisItems
            .Include(pai => pai.OrderSamplePackage)
            .Include(pai => pai.AnalysisItem)
            .FirstOrDefault(pai => pai.OrderSamplePackageAnalysisItemId == key);
        if (row == null)
        {
            return NotFound();
        }

        return Ok(row);
    }

    [HttpPost("OrderSamplePackageAnalysisItems")]
    public async Task<IActionResult> Post([FromBody] OrderSamplePackageAnalysisItem row)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.OrderSamplePackageAnalysisItemId = row.OrderSamplePackageAnalysisItemId == Guid.Empty
            ? Guid.NewGuid()
            : row.OrderSamplePackageAnalysisItemId;
        row.CreatedAt = DateTime.UtcNow;
        _context.OrderSamplePackageAnalysisItems.Add(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu trong gói mẫu đơn hàng");
        }

        return Created($"odata/OrderSamplePackageAnalysisItems({row.OrderSamplePackageAnalysisItemId})", row);
    }

    [HttpPut("OrderSamplePackageAnalysisItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderSamplePackageAnalysisItem row)
    {
        if (key != row.OrderSamplePackageAnalysisItemId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(row).State = EntityState.Modified;

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
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu trong gói mẫu đơn hàng");
        }

        return Updated(row);
    }

    [HttpDelete("OrderSamplePackageAnalysisItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.OrderSamplePackageAnalysisItems.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.OrderSamplePackageAnalysisItems.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu trong gói mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderSamplePackageAnalysisItems.Any(e => e.OrderSamplePackageAnalysisItemId == key);
    }
}
