using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
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
public class SubcontractorCapabilitiesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubcontractorCapabilitiesController> _logger;

    public SubcontractorCapabilitiesController(
        ApplicationDbContext context,
        ILogger<SubcontractorCapabilitiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("SubcontractorCapabilities")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.SubcontractorCapabilities
            .Include(sc => sc.Subcontractor)
            .Include(sc => sc.AnalysisItem));
    }

    [HttpGet("SubcontractorCapabilities({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.SubcontractorCapabilities
            .Include(sc => sc.Subcontractor)
            .Include(sc => sc.AnalysisItem)
            .FirstOrDefault(sc => sc.SubcontractorCapabilityId == key);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost("SubcontractorCapabilities")]
    public async Task<IActionResult> Post([FromBody] SubcontractorCapability item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subcontractorExists = await _context.Subcontractors.AnyAsync(s => s.SubcontractorId == item.SubcontractorId);
        if (!subcontractorExists)
            return BadRequest("Nhà thầu phụ không tồn tại.");

        var analysisItemExists = await _context.AnalysisItems.AnyAsync(a => a.AnalysisItemId == item.AnalysisItemId);
        if (!analysisItemExists)
            return BadRequest("Chỉ tiêu phân tích không tồn tại.");

        var duplicate = await _context.SubcontractorCapabilities
            .AnyAsync(sc => sc.SubcontractorId == item.SubcontractorId && sc.AnalysisItemId == item.AnalysisItemId);
        if (duplicate)
            return BadRequest("Cặp nhà thầu phụ - chỉ tiêu này đã tồn tại.");

        item.SubcontractorCapabilityId = item.SubcontractorCapabilityId == Guid.Empty ? Guid.NewGuid() : item.SubcontractorCapabilityId;
        if (string.IsNullOrEmpty(item.Status))
            item.Status = "Active";
        item.CreatedAt = DateTime.UtcNow;

        _context.SubcontractorCapabilities.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu năng lực nhà thầu phụ");
        }

        return Created($"odata/SubcontractorCapabilities({item.SubcontractorCapabilityId})", item);
    }

    [HttpPut("SubcontractorCapabilities({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] SubcontractorCapability item)
    {
        if (key != item.SubcontractorCapabilityId)
            return BadRequest("Key mismatch");
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subcontractorExists = await _context.Subcontractors.AnyAsync(s => s.SubcontractorId == item.SubcontractorId);
        if (!subcontractorExists)
            return BadRequest("Nhà thầu phụ không tồn tại.");

        var analysisItemExists = await _context.AnalysisItems.AnyAsync(a => a.AnalysisItemId == item.AnalysisItemId);
        if (!analysisItemExists)
            return BadRequest("Chỉ tiêu phân tích không tồn tại.");

        var duplicate = await _context.SubcontractorCapabilities
            .AnyAsync(sc => sc.SubcontractorId == item.SubcontractorId && sc.AnalysisItemId == item.AnalysisItemId && sc.SubcontractorCapabilityId != key);
        if (duplicate)
            return BadRequest("Cặp nhà thầu phụ - chỉ tiêu này đã tồn tại.");

        item.UpdatedAt = DateTime.UtcNow;
        _context.Entry(item).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubcontractorCapabilityExists(key))
                return NotFound();
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật năng lực nhà thầu phụ");
        }

        return Updated(item);
    }

    [HttpPatch("SubcontractorCapabilities({key})")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] Delta<SubcontractorCapability> patch)
    {
        var item = await _context.SubcontractorCapabilities.FindAsync(key);
        if (item == null)
            return NotFound();

        patch.Patch(item);
        item.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật năng lực nhà thầu phụ");
        }

        return Updated(item);
    }

    [HttpDelete("SubcontractorCapabilities({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.SubcontractorCapabilities.FindAsync(key);
        if (item == null)
            return NotFound();

        _context.SubcontractorCapabilities.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa năng lực nhà thầu phụ");
        }

        return NoContent();
    }

    private bool SubcontractorCapabilityExists(Guid key)
        => _context.SubcontractorCapabilities.Any(e => e.SubcontractorCapabilityId == key);
}
