using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class EmployeesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Employees")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Employees);
    }

    [HttpGet("Employees({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == key);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    [HttpPost("Employees")]
    public async Task<IActionResult> Post([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        employee.EmployeeId = employee.EmployeeId == Guid.Empty ? Guid.NewGuid() : employee.EmployeeId;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Created($"odata/Employees({employee.EmployeeId})", employee);
    }

    [HttpPut("Employees({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Employee employee)
    {
        if (key != employee.EmployeeId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(employee);
    }

    [HttpDelete("Employees({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var employee = await _context.Employees.FindAsync(key);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(Guid key)
    {
        return _context.Employees.Any(e => e.EmployeeId == key);
    }
}


