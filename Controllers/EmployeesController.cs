using System.Security.Claims;
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
public class EmployeesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("Employees")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Employees
            .Include(e => e.Department)
            .Include(e => e.EmployeeTitle)
            .Include(e => e.UpdatedByAccount)
            .Include(e => e.Account).ThenInclude(a => a!.Permission));
    }

    [HttpGet("Employees({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var employee = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.EmployeeTitle)
            .Include(e => e.UpdatedByAccount)
            .Include(e => e.Account).ThenInclude(a => a!.Permission)
            .FirstOrDefault(e => e.EmployeeId == key);
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhân viên");
        }

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

        var existing = await _context.Employees.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.EmployeeCode = employee.EmployeeCode;
        existing.DepartmentId = employee.DepartmentId;
        existing.Role = employee.Role;
        existing.FullName = employee.FullName;
        existing.EmployeeTitleId = employee.EmployeeTitleId;
        existing.Title = employee.Title;
        existing.Email = employee.Email;
        existing.Mobile = employee.Mobile;
        existing.Notes = employee.Notes;
        existing.Status = employee.Status;
        existing.ManagerId = employee.ManagerId;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhân viên");
        }

        var updated = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.EmployeeTitle)
            .Include(e => e.UpdatedByAccount)
            .Include(e => e.Account).ThenInclude(a => a!.Permission)
            .FirstOrDefaultAsync(e => e.EmployeeId == key);
        return Updated(updated ?? existing);
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhân viên");
        }

        return NoContent();
    }

    private bool EmployeeExists(Guid key)
    {
        return _context.Employees.Any(e => e.EmployeeId == key);
    }
}


