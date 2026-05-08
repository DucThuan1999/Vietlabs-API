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
public class OrdersController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Orders")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Orders
            .Include(o => o.Client)
            .Include(o => o.Contact)
            .Include(o => o.CreatedByAccount));
    }

    [HttpGet("Orders({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var order = _context.Orders
            .Include(o => o.Client)
            .Include(o => o.Contact)
            .Include(o => o.CreatedByAccount)
            .FirstOrDefault(o => o.OrderId == key);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost("Orders")]
    public async Task<IActionResult> Post([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        order.OrderId = order.OrderId == Guid.Empty ? Guid.NewGuid() : order.OrderId;
        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu đơn hàng");
        }

        var createdOrder = await _context.Orders
            .Include(o => o.Client)
            .Include(o => o.Contact)
            .Include(o => o.CreatedByAccount)
            .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

        return Created($"odata/Orders({order.OrderId})", createdOrder);
    }

    [HttpPut("Orders({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Order order)
    {
        if (key != order.OrderId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(key))
            {
                return NotFound();
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật đơn hàng");
        }

        var updatedOrder = await _context.Orders
            .Include(o => o.Client)
            .Include(o => o.Contact)
            .Include(o => o.CreatedByAccount)
            .FirstOrDefaultAsync(o => o.OrderId == key);

        return Updated(updatedOrder);
    }

    [HttpDelete("Orders({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var order = await _context.Orders.FindAsync(key);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa đơn hàng");
        }

        return NoContent();
    }

    private bool OrderExists(Guid key)
    {
        return _context.Orders.Any(e => e.OrderId == key);
    }
}
