using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class DepartmentAnalysisCapabilitiesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public DepartmentAnalysisCapabilitiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("DepartmentAnalysisCapabilities")]
    [EnableQuery]
    public IActionResult Get()
    {
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
        await _context.SaveChangesAsync();

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
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DepartmentAnalysisCapabilityExists(Guid key)
    {
        return _context.DepartmentAnalysisCapabilities.Any(e => e.DepartmentAnalysisCapabilityId == key);
    }
}

