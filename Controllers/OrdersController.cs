using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Data.Queries;
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
        return Ok(QueryOrders());
    }

    [HttpGet("Orders({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var order = QueryOrders().FirstOrDefault(o => o.OrderId == key);

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

        var validationError = await OrderLinkValidator.ValidateForCreateAsync(_context, order);
        if (validationError != null)
        {
            return BadRequest(validationError);
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

        var createdOrder = await QueryOrders()
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

        var existing = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == key);
        if (existing == null)
        {
            return NotFound();
        }

        var validationError = await OrderLinkValidator.ValidateForUpdateAsync(_context, existing, order);
        if (validationError != null)
        {
            return BadRequest(validationError);
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

        var updatedOrder = await QueryOrders()
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

        var childOrders = await _context.Orders
            .Where(o => o.ParentOrderId == key)
            .ToListAsync();
        _context.Orders.RemoveRange(childOrders);
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

    private IQueryable<Order> QueryOrders()
    {
        return _context.Orders
            .Include(o => o.Client)
            .Include(o => o.Contact)
            .Include(o => o.CreatedByAccount)
            .Include(o => o.ParentOrder)
            .WithLinkedOrderCount(_context);
    }

    private bool OrderExists(Guid key)
    {
        return _context.Orders.Any(e => e.OrderId == key);
    }
}
