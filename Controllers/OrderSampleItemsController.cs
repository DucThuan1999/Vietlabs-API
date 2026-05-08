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
public class OrderSampleItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderSampleItemsController> _logger;

    public OrderSampleItemsController(ApplicationDbContext context, ILogger<OrderSampleItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderSampleItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderSampleItems
            .Include(qi => qi.OrderSample)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package));
    }

    [HttpGet("OrderSampleItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.OrderSampleItems
            .Include(qi => qi.OrderSample)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package)
            .FirstOrDefault(qi => qi.OrderSampleItemId == key);
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost("OrderSampleItems")]
    public async Task<IActionResult> Post([FromBody] OrderSampleItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.OrderSampleItemId = item.OrderSampleItemId == Guid.Empty
            ? Guid.NewGuid()
            : item.OrderSampleItemId;
        item.CreatedAt = DateTime.UtcNow;
        _context.OrderSampleItems.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu mẫu đơn hàng");
        }

        return Created($"odata/OrderSampleItems({item.OrderSampleItemId})", item);
    }

    [HttpPut("OrderSampleItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderSampleItem item)
    {
        if (key != item.OrderSampleItemId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.UpdatedAt = DateTime.UtcNow;
        _context.Entry(item).State = EntityState.Modified;

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
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu mẫu đơn hàng");
        }

        return Updated(item);
    }

    [HttpDelete("OrderSampleItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.OrderSampleItems.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.OrderSampleItems.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderSampleItems.Any(e => e.OrderSampleItemId == key);
    }
}
