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
public class OrderTemplateItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderTemplateItemsController> _logger;

    public OrderTemplateItemsController(ApplicationDbContext context, ILogger<OrderTemplateItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderTemplateItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderTemplateItems
            .Include(qi => qi.OrderTemplate)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package));
    }

    [HttpGet("OrderTemplateItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.OrderTemplateItems
            .Include(qi => qi.OrderTemplate)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package)
            .FirstOrDefault(qi => qi.OrderTemplateItemId == key);
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost("OrderTemplateItems")]
    public async Task<IActionResult> Post([FromBody] OrderTemplateItem item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        item.OrderTemplateItemId = item.OrderTemplateItemId == Guid.Empty
            ? Guid.NewGuid()
            : item.OrderTemplateItemId;
        item.CreatedAt = DateTime.UtcNow;
        _context.OrderTemplateItems.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu template mẫu đơn hàng");
        }

        return Created($"odata/OrderTemplateItems({item.OrderTemplateItemId})", item);
    }

    [HttpPut("OrderTemplateItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderTemplateItem item)
    {
        if (key != item.OrderTemplateItemId)
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu template mẫu đơn hàng");
        }

        return Updated(item);
    }

    [HttpDelete("OrderTemplateItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.OrderTemplateItems.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.OrderTemplateItems.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu template mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderTemplateItems.Any(e => e.OrderTemplateItemId == key);
    }
}
