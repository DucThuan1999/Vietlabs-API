using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class DepartmentsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Departments")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Departments);
    }

    [HttpGet("Departments({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var dept = _context.Departments.FirstOrDefault(d => d.DepartmentId == key);
        if (dept == null)
        {
            return NotFound();
        }
        return Ok(dept);
    }

    [HttpPost("Departments")]
    public async Task<IActionResult> Post([FromBody] Department dept)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        dept.DepartmentId = dept.DepartmentId == Guid.Empty ? Guid.NewGuid() : dept.DepartmentId;
        _context.Departments.Add(dept);
        await _context.SaveChangesAsync();

        return Created($"odata/Departments({dept.DepartmentId})", dept);
    }

    [HttpPut("Departments({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Department dept)
    {
        if (key != dept.DepartmentId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(dept).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DepartmentExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(dept);
    }

    [HttpDelete("Departments({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var dept = await _context.Departments.FindAsync(key);
        if (dept == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(dept);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DepartmentExists(Guid key)
    {
        return _context.Departments.Any(d => d.DepartmentId == key);
    }
}


