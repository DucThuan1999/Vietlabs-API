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
public class DepartmentAnalysisCapabilitiesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DepartmentAnalysisCapabilitiesController> _logger;

    public DepartmentAnalysisCapabilitiesController(ApplicationDbContext context, ILogger<DepartmentAnalysisCapabilitiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("DepartmentAnalysisCapabilities")]
    [EnableQuery]
    public IActionResult Get()
    {
        // Không Include Designations trên list — client dùng $expand hoặc bảng DepartmentAnalysisCapabilityDesignations.
        return Ok(_context.DepartmentAnalysisCapabilities
            .Include(dac => dac.Department)
            .Include(dac => dac.AnalysisItem));
    }

    [HttpGet("DepartmentAnalysisCapabilities({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var departmentAnalysisCapability = _context.DepartmentAnalysisCapabilities
            .Include(dac => dac.Department)
            .Include(dac => dac.AnalysisItem)
                .ThenInclude(ai => ai!.SampleMatrix)
            .Include(dac => dac.Designations).ThenInclude(d => d.Designation)
            .FirstOrDefault(dac => dac.DepartmentAnalysisCapabilityId == key);
        if (departmentAnalysisCapability == null)
        {
            return NotFound();
        }
        return Ok(departmentAnalysisCapability);
    }

    [HttpPost("DepartmentAnalysisCapabilities")]
    public async Task<IActionResult> Post([FromBody] DepartmentAnalysisCapability departmentAnalysisCapability)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        departmentAnalysisCapability.DepartmentAnalysisCapabilityId = 
            departmentAnalysisCapability.DepartmentAnalysisCapabilityId == Guid.Empty 
            ? Guid.NewGuid() 
            : departmentAnalysisCapability.DepartmentAnalysisCapabilityId;
        departmentAnalysisCapability.CreatedAt = DateTime.UtcNow;
        _context.DepartmentAnalysisCapabilities.Add(departmentAnalysisCapability);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu năng lực phân tích phòng ban");
        }

        return Created($"odata/DepartmentAnalysisCapabilities({departmentAnalysisCapability.DepartmentAnalysisCapabilityId})", 
            departmentAnalysisCapability);
    }

    [HttpPut("DepartmentAnalysisCapabilities({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] DepartmentAnalysisCapability departmentAnalysisCapability)
    {
        if (key != departmentAnalysisCapability.DepartmentAnalysisCapabilityId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        departmentAnalysisCapability.UpdatedAt = DateTime.UtcNow;
        _context.Entry(departmentAnalysisCapability).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DepartmentAnalysisCapabilityExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật năng lực phân tích phòng ban");
        }

        return Updated(departmentAnalysisCapability);
    }

    [HttpDelete("DepartmentAnalysisCapabilities({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var departmentAnalysisCapability = await _context.DepartmentAnalysisCapabilities.FindAsync(key);
        if (departmentAnalysisCapability == null)
        {
            return NotFound();
        }

        _context.DepartmentAnalysisCapabilities.Remove(departmentAnalysisCapability);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa năng lực phân tích phòng ban");
        }

        return NoContent();
    }

    private bool DepartmentAnalysisCapabilityExists(Guid key)
    {
        return _context.DepartmentAnalysisCapabilities.Any(e => e.DepartmentAnalysisCapabilityId == key);
    }
}

