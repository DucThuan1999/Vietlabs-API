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
public class QuotationAnalysisGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationAnalysisGroupsController> _logger;

    public QuotationAnalysisGroupsController(ApplicationDbContext context, ILogger<QuotationAnalysisGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationAnalysisGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationAnalysisGroups
            .Include(qag => qag.Quotation)
            .Include(qag => qag.AnalysisGroup));
    }

    [HttpGet("QuotationAnalysisGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var quotationAnalysisGroup = _context.QuotationAnalysisGroups
            .Include(qag => qag.Quotation)
            .Include(qag => qag.AnalysisGroup)
            .FirstOrDefault(qag => qag.QuotationAnalysisGroupId == key);
        if (quotationAnalysisGroup == null)
        {
            return NotFound();
        }
        return Ok(quotationAnalysisGroup);
    }

    [HttpPost("QuotationAnalysisGroups")]
    public async Task<IActionResult> Post([FromBody] QuotationAnalysisGroup quotationAnalysisGroup)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotationAnalysisGroup.QuotationAnalysisGroupId = quotationAnalysisGroup.QuotationAnalysisGroupId == Guid.Empty 
            ? Guid.NewGuid() 
            : quotationAnalysisGroup.QuotationAnalysisGroupId;
        quotationAnalysisGroup.CreatedAt = DateTime.UtcNow;
        _context.QuotationAnalysisGroups.Add(quotationAnalysisGroup);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhóm phân tích báo giá");
        }

        return Created($"odata/QuotationAnalysisGroups({quotationAnalysisGroup.QuotationAnalysisGroupId})", quotationAnalysisGroup);
    }

    [HttpPut("QuotationAnalysisGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationAnalysisGroup quotationAnalysisGroup)
    {
        if (key != quotationAnalysisGroup.QuotationAnalysisGroupId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotationAnalysisGroup.UpdatedAt = DateTime.UtcNow;
        _context.Entry(quotationAnalysisGroup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationAnalysisGroupExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhóm phân tích báo giá");
        }

        return Updated(quotationAnalysisGroup);
    }

    [HttpDelete("QuotationAnalysisGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var quotationAnalysisGroup = await _context.QuotationAnalysisGroups.FindAsync(key);
        if (quotationAnalysisGroup == null)
        {
            return NotFound();
        }

        _context.QuotationAnalysisGroups.Remove(quotationAnalysisGroup);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhóm phân tích báo giá");
        }

        return NoContent();
    }

    private bool QuotationAnalysisGroupExists(Guid key)
    {
        return _context.QuotationAnalysisGroups.Any(e => e.QuotationAnalysisGroupId == key);
    }
}

