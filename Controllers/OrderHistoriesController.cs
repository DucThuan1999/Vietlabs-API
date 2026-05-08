using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Helpers;
using VietLab.Models;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class OrderHistoriesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderHistoriesController> _logger;

    public OrderHistoriesController(ApplicationDbContext context, ILogger<OrderHistoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderHistories")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderHistories
            .Include(oh => oh.Order)
            .Include(oh => oh.CreatedByAccount)
                .ThenInclude(a => a!.Employee));
    }

    [HttpGet("OrderHistories({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var history = _context.OrderHistories
            .Include(oh => oh.Order)
            .Include(oh => oh.CreatedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefault(oh => oh.OrderHistoryId == key);

        if (history == null)
        {
            return NotFound();
        }

        return Ok(history);
    }

    [HttpPost("OrderHistories")]
    public async Task<IActionResult> Post([FromBody] OrderHistory orderHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderHistory.OrderId);
        if (!orderExists)
        {
            return BadRequest($"Order with ID {orderHistory.OrderId} does not exist.");
        }

        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == orderHistory.CreatedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {orderHistory.CreatedByAccountId} does not exist.");
        }

        orderHistory.OrderHistoryId = orderHistory.OrderHistoryId == Guid.Empty ? Guid.NewGuid() : orderHistory.OrderHistoryId;
        if (orderHistory.ActivityDate == default)
        {
            orderHistory.ActivityDate = DateTime.UtcNow;
        }

        if (string.IsNullOrWhiteSpace(orderHistory.Activity))
        {
            return BadRequest("Activity is required.");
        }

        _context.OrderHistories.Add(orderHistory);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu lịch sử đơn hàng");
        }

        var created = await _context.OrderHistories
            .Include(oh => oh.Order)
            .Include(oh => oh.CreatedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(oh => oh.OrderHistoryId == orderHistory.OrderHistoryId);

        return Created($"odata/OrderHistories({orderHistory.OrderHistoryId})", created);
    }

    [HttpPut("OrderHistories({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderHistory orderHistory)
    {
        if (key != orderHistory.OrderHistoryId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.OrderHistories.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderHistory.OrderId);
        if (!orderExists)
        {
            return BadRequest($"Order with ID {orderHistory.OrderId} does not exist.");
        }

        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == orderHistory.CreatedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {orderHistory.CreatedByAccountId} does not exist.");
        }

        if (string.IsNullOrWhiteSpace(orderHistory.Activity))
        {
            return BadRequest("Activity is required.");
        }

        _context.Entry(existing).State = EntityState.Detached;
        _context.Entry(orderHistory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderHistoryExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật lịch sử đơn hàng");
        }

        var updated = await _context.OrderHistories
            .Include(oh => oh.Order)
            .Include(oh => oh.CreatedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(oh => oh.OrderHistoryId == key);

        return Updated(updated);
    }

    [HttpDelete("OrderHistories({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var history = await _context.OrderHistories.FindAsync(key);
        if (history == null)
        {
            return NotFound();
        }

        _context.OrderHistories.Remove(history);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa lịch sử đơn hàng");
        }

        return NoContent();
    }

    private bool OrderHistoryExists(Guid key)
    {
        return _context.OrderHistories.Any(e => e.OrderHistoryId == key);
    }
}

