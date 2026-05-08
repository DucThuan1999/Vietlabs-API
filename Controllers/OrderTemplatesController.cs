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
public class OrderTemplatesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderTemplatesController> _logger;

    public OrderTemplatesController(ApplicationDbContext context, ILogger<OrderTemplatesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderTemplates")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderTemplates
            .Include(t => t.OrderSample)
            .Include(t => t.CreatedByAccount));
    }

    [HttpGet("OrderTemplates({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.OrderTemplates
            .Include(t => t.OrderSample)
            .Include(t => t.CreatedByAccount)
            .FirstOrDefault(t => t.TemplateId == key);
        if (row == null)
        {
            return NotFound();
        }

        return Ok(row);
    }

    [HttpPost("OrderTemplates")]
    public async Task<IActionResult> Post([FromBody] OrderTemplate row)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.TemplateId = row.TemplateId == Guid.Empty ? Guid.NewGuid() : row.TemplateId;
        row.CreatedAt = DateTime.UtcNow;

        _context.OrderTemplates.Add(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu template mẫu đơn hàng");
        }

        return Created($"odata/OrderTemplates({row.TemplateId})", row);
    }

    [HttpPut("OrderTemplates({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderTemplate row)
    {
        if (key != row.TemplateId)
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật template mẫu đơn hàng");
        }

        return Updated(row);
    }

    [HttpDelete("OrderTemplates({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.OrderTemplates.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.OrderTemplates.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa template mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderTemplates.Any(e => e.TemplateId == key);
    }
}

