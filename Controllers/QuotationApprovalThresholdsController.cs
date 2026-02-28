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
public class QuotationApprovalThresholdsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationApprovalThresholdsController> _logger;

    public QuotationApprovalThresholdsController(ApplicationDbContext context, ILogger<QuotationApprovalThresholdsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationApprovalThresholds")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationApprovalThresholds);
    }

    [HttpGet("QuotationApprovalThresholds({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var threshold = _context.QuotationApprovalThresholds
            .FirstOrDefault(t => t.QuotationApprovalThresholdId == key);
        if (threshold == null)
        {
            return NotFound();
        }
        return Ok(threshold);
    }

    [HttpPost("QuotationApprovalThresholds")]
    public async Task<IActionResult> Post([FromBody] QuotationApprovalThreshold threshold)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        threshold.QuotationApprovalThresholdId = threshold.QuotationApprovalThresholdId == Guid.Empty 
            ? Guid.NewGuid() 
            : threshold.QuotationApprovalThresholdId;
        threshold.CreatedAt = DateTime.UtcNow;
        
        _context.QuotationApprovalThresholds.Add(threshold);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu ngưỡng phê duyệt báo giá");
        }

        return Created($"odata/QuotationApprovalThresholds({threshold.QuotationApprovalThresholdId})", threshold);
    }

    [HttpPut("QuotationApprovalThresholds({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationApprovalThreshold threshold)
    {
        if (key != threshold.QuotationApprovalThresholdId)
        {
            return BadRequest(new { 
                error = "Key mismatch",
                message = $"The key in URL ({key}) does not match QuotationApprovalThresholdId in body ({threshold.QuotationApprovalThresholdId})"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        threshold.UpdatedAt = DateTime.UtcNow;
        _context.Entry(threshold).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationApprovalThresholdExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật ngưỡng phê duyệt báo giá");
        }

        return Updated(threshold);
    }

    [HttpDelete("QuotationApprovalThresholds({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var threshold = await _context.QuotationApprovalThresholds.FindAsync(key);
        if (threshold == null)
        {
            return NotFound();
        }

        _context.QuotationApprovalThresholds.Remove(threshold);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa ngưỡng phê duyệt báo giá");
        }

        return NoContent();
    }

    private bool QuotationApprovalThresholdExists(Guid key)
    {
        return _context.QuotationApprovalThresholds.Any(e => e.QuotationApprovalThresholdId == key);
    }
}

