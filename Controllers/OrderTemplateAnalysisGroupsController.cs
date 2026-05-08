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
public class OrderTemplateAnalysisGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderTemplateAnalysisGroupsController> _logger;

    public OrderTemplateAnalysisGroupsController(ApplicationDbContext context, ILogger<OrderTemplateAnalysisGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderTemplateAnalysisGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderTemplateAnalysisGroups
            .Include(qag => qag.OrderTemplate)
            .Include(qag => qag.AnalysisGroup));
    }

    [HttpGet("OrderTemplateAnalysisGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.OrderTemplateAnalysisGroups
            .Include(qag => qag.OrderTemplate)
            .Include(qag => qag.AnalysisGroup)
            .FirstOrDefault(qag => qag.OrderTemplateAnalysisGroupId == key);
        if (row == null)
        {
            return NotFound();
        }

        return Ok(row);
    }

    [HttpPost("OrderTemplateAnalysisGroups")]
    public async Task<IActionResult> Post([FromBody] OrderTemplateAnalysisGroup row)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        row.OrderTemplateAnalysisGroupId = row.OrderTemplateAnalysisGroupId == Guid.Empty
            ? Guid.NewGuid()
            : row.OrderTemplateAnalysisGroupId;
        row.CreatedAt = DateTime.UtcNow;
        _context.OrderTemplateAnalysisGroups.Add(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhóm chỉ tiêu template mẫu đơn hàng");
        }

        return Created($"odata/OrderTemplateAnalysisGroups({row.OrderTemplateAnalysisGroupId})", row);
    }

    [HttpPut("OrderTemplateAnalysisGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderTemplateAnalysisGroup row)
    {
        if (key != row.OrderTemplateAnalysisGroupId)
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhóm chỉ tiêu template mẫu đơn hàng");
        }

        return Updated(row);
    }

    [HttpDelete("OrderTemplateAnalysisGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.OrderTemplateAnalysisGroups.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.OrderTemplateAnalysisGroups.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhóm chỉ tiêu template mẫu đơn hàng");
        }

        return NoContent();
    }

    private bool Exists(Guid key)
    {
        return _context.OrderTemplateAnalysisGroups.Any(e => e.OrderTemplateAnalysisGroupId == key);
    }
}
