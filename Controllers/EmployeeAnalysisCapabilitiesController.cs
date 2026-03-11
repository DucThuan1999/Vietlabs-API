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
public class EmployeeAnalysisCapabilitiesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeAnalysisCapabilitiesController> _logger;

    public EmployeeAnalysisCapabilitiesController(
        ApplicationDbContext context,
        ILogger<EmployeeAnalysisCapabilitiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("EmployeeAnalysisCapabilities")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.EmployeeAnalysisCapabilities
            .Include(eac => eac.Employee)
            .Include(eac => eac.AnalysisItem));
    }

    [HttpGet("EmployeeAnalysisCapabilities({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.EmployeeAnalysisCapabilities
            .Include(eac => eac.Employee)
            .Include(eac => eac.AnalysisItem)
            .FirstOrDefault(eac => eac.EmployeeAnalysisCapabilityId == key);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost("EmployeeAnalysisCapabilities")]
    public async Task<IActionResult> Post([FromBody] EmployeeAnalysisCapability item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        item.EmployeeAnalysisCapabilityId = item.EmployeeAnalysisCapabilityId == Guid.Empty
            ? Guid.NewGuid()
            : item.EmployeeAnalysisCapabilityId;
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        _context.EmployeeAnalysisCapabilities.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu năng lực nhân viên");
        }

        return Created($"odata/EmployeeAnalysisCapabilities({item.EmployeeAnalysisCapabilityId})", item);
    }

    [HttpPut("EmployeeAnalysisCapabilities({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] EmployeeAnalysisCapability item)
    {
        if (key != item.EmployeeAnalysisCapabilityId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.EmployeeAnalysisCapabilities.FindAsync(key);
        if (existing == null)
            return NotFound();

        existing.EmployeeId = item.EmployeeId;
        existing.AnalysisItemId = item.AnalysisItemId;
        existing.Status = item.Status;
        existing.Notes = item.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = item.UpdatedBy;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật năng lực nhân viên");
        }

        return Updated(existing);
    }

    [HttpDelete("EmployeeAnalysisCapabilities({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.EmployeeAnalysisCapabilities.FindAsync(key);
        if (item == null)
            return NotFound();

        _context.EmployeeAnalysisCapabilities.Remove(item);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa năng lực nhân viên");
        }
        return NoContent();
    }
}
