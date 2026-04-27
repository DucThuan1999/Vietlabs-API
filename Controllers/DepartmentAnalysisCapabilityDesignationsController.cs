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
public class DepartmentAnalysisCapabilityDesignationsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DepartmentAnalysisCapabilityDesignationsController> _logger;

    public DepartmentAnalysisCapabilityDesignationsController(
        ApplicationDbContext context,
        ILogger<DepartmentAnalysisCapabilityDesignationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("DepartmentAnalysisCapabilityDesignations")]
    [EnableQuery]
    public IActionResult Get()
    {
        // List: chỉ Include Designation — tránh nạp toàn bộ DepartmentAnalysisCapability × N (báo giá / OData list).
        return Ok(_context.DepartmentAnalysisCapabilityDesignations
            .Include(dacd => dacd.Designation));
    }

    [HttpGet("DepartmentAnalysisCapabilityDesignations({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.DepartmentAnalysisCapabilityDesignations
            .Include(dacd => dacd.DepartmentAnalysisCapability)
            .Include(dacd => dacd.Designation)
            .FirstOrDefault(dacd => dacd.DepartmentAnalysisCapabilityDesignationId == key);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost("DepartmentAnalysisCapabilityDesignations")]
    public async Task<IActionResult> Post([FromBody] DepartmentAnalysisCapabilityDesignation item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        item.DepartmentAnalysisCapabilityDesignationId = item.DepartmentAnalysisCapabilityDesignationId == Guid.Empty
            ? Guid.NewGuid()
            : item.DepartmentAnalysisCapabilityDesignationId;
        _context.DepartmentAnalysisCapabilityDesignations.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ định năng lực phòng ban");
        }

        return Created($"odata/DepartmentAnalysisCapabilityDesignations({item.DepartmentAnalysisCapabilityDesignationId})", item);
    }

    [HttpPut("DepartmentAnalysisCapabilityDesignations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] DepartmentAnalysisCapabilityDesignation item)
    {
        if (key != item.DepartmentAnalysisCapabilityDesignationId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.DepartmentAnalysisCapabilityDesignations.FindAsync(key);
        if (existing == null)
            return NotFound();

        existing.DesignationId = item.DesignationId;
        existing.ExpiredDate = item.ExpiredDate;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ định năng lực phòng ban");
        }

        await _context.Entry(existing).Reference(dacd => dacd.Designation).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("DepartmentAnalysisCapabilityDesignations({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.DepartmentAnalysisCapabilityDesignations.FindAsync(key);
        if (item == null)
            return NotFound();

        _context.DepartmentAnalysisCapabilityDesignations.Remove(item);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ định năng lực phòng ban");
        }
        return NoContent();
    }
}
