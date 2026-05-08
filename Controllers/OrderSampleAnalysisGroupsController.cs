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
public class OrderSampleAnalysisGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderSampleAnalysisGroupsController> _logger;

    public OrderSampleAnalysisGroupsController(ApplicationDbContext context, ILogger<OrderSampleAnalysisGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderSampleAnalysisGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderSampleAnalysisGroups
            .Include(qag => qag.OrderSample)
            .Include(qag => qag.AnalysisGroup));
    }

    [HttpGet("OrderSampleAnalysisGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.OrderSampleAnalysisGroups
            .Include(qag => qag.OrderSample)
            .Include(qag => qag.AnalysisGroup)
            .FirstOrDefault(qag => qag.OrderSampleAnalysisGroupId == key);
        if (row == null)
        {
            return NotFound();
        }

        return Ok(row);
    }

    [HttpPost("OrderSampleAnalysisGroups")]
    public async Task<IActionResult> Post([FromBody] OrderSampleAnalysisGroup row)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.OrderSampleAnalysisGroupId = row.OrderSampleAnalysisGroupId == Guid.Empty
            ? Guid.NewGuid()
            : row.OrderSampleAnalysisGroupId;
        row.CreatedAt = DateTime.UtcNow;
        _context.OrderSampleAnalysisGroups.Add(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhóm chỉ tiêu mẫu đơn hàng");
        }

        return Created($"odata/OrderSampleAnalysisGroups({row.OrderSampleAnalysisGroupId})", row);
    }

    [HttpPut("OrderSampleAnalysisGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderSampleAnalysisGroup row)
    {
        if (key != row.OrderSampleAnalysisGroupId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.UpdatedAt = DateTime.UtcNow;
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhóm chỉ tiêu mẫu đơn hàng");
        }

        return Updated(row);
    }

    [HttpDelete("OrderSampleAnalysisGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.OrderSampleAnalysisGroups.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.OrderSampleAnalysisGroups.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhóm chỉ tiêu mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderSampleAnalysisGroups.Any(e => e.OrderSampleAnalysisGroupId == key);
    }
}
