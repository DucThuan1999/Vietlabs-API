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
public class OrderTemplatePackageAnalysisItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderTemplatePackageAnalysisItemsController> _logger;

    public OrderTemplatePackageAnalysisItemsController(ApplicationDbContext context, ILogger<OrderTemplatePackageAnalysisItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderTemplatePackageAnalysisItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderTemplatePackageAnalysisItems
            .Include(pai => pai.OrderTemplatePackage)
            .Include(pai => pai.AnalysisItem));
    }

    [HttpGet("OrderTemplatePackageAnalysisItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.OrderTemplatePackageAnalysisItems
            .Include(pai => pai.OrderTemplatePackage)
            .Include(pai => pai.AnalysisItem)
            .FirstOrDefault(pai => pai.OrderTemplatePackageAnalysisItemId == key);
        if (row == null)
        {
            return NotFound();
        }

        return Ok(row);
    }

    [HttpPost("OrderTemplatePackageAnalysisItems")]
    public async Task<IActionResult> Post([FromBody] OrderTemplatePackageAnalysisItem row)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.OrderTemplatePackageAnalysisItemId = row.OrderTemplatePackageAnalysisItemId == Guid.Empty
            ? Guid.NewGuid()
            : row.OrderTemplatePackageAnalysisItemId;
        row.CreatedAt = DateTime.UtcNow;
        _context.OrderTemplatePackageAnalysisItems.Add(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu trong gói template mẫu đơn hàng");
        }

        return Created($"odata/OrderTemplatePackageAnalysisItems({row.OrderTemplatePackageAnalysisItemId})", row);
    }

    [HttpPut("OrderTemplatePackageAnalysisItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderTemplatePackageAnalysisItem row)
    {
        if (key != row.OrderTemplatePackageAnalysisItemId)
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ tiêu trong gói template mẫu đơn hàng");
        }

        return Updated(row);
    }

    [HttpDelete("OrderTemplatePackageAnalysisItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.OrderTemplatePackageAnalysisItems.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.OrderTemplatePackageAnalysisItems.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu trong gói template mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderTemplatePackageAnalysisItems.Any(e => e.OrderTemplatePackageAnalysisItemId == key);
    }
}
