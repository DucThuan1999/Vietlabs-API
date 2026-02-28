using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class EmployeeTitlesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeTitlesController> _logger;

    public EmployeeTitlesController(
        ApplicationDbContext context,
        ILogger<EmployeeTitlesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("EmployeeTitles")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.EmployeeTitles
            .Include(e => e.CreatedByAccount)
            .Include(e => e.UpdatedByAccount));
    }

    [HttpGet("EmployeeTitles({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var title = _context.EmployeeTitles
            .Include(e => e.CreatedByAccount)
            .Include(e => e.UpdatedByAccount)
            .FirstOrDefault(e => e.EmployeeTitleId == key);
        if (title == null)
        {
            return NotFound();
        }
        return Ok(title);
    }

    [HttpPost("EmployeeTitles")]
    public async Task<IActionResult> Post([FromBody] EmployeeTitle title)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        title.EmployeeTitleId = title.EmployeeTitleId == Guid.Empty ? Guid.NewGuid() : title.EmployeeTitleId;
        if (string.IsNullOrEmpty(title.Status))
        {
            title.Status = "Active";
        }
        title.CreatedAt = DateTime.UtcNow;
        title.CreatedBy = GetCurrentAccountId();

        _context.EmployeeTitles.Add(title);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chức vụ nhân viên");
        }

        return Created($"odata/EmployeeTitles({title.EmployeeTitleId})", title);
    }

    [HttpPut("EmployeeTitles({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] EmployeeTitle title)
    {
        if (key != title.EmployeeTitleId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        title.UpdatedAt = DateTime.UtcNow;
        title.UpdatedBy = GetCurrentAccountId();
        _context.Entry(title).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeTitleExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chức vụ nhân viên");
        }

        return Updated(title);
    }

    [HttpPatch("EmployeeTitles({key})")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] Microsoft.AspNetCore.OData.Deltas.Delta<EmployeeTitle> patch)
    {
        var title = await _context.EmployeeTitles.FindAsync(key);
        if (title == null)
        {
            return NotFound();
        }

        patch.Patch(title);
        title.UpdatedAt = DateTime.UtcNow;
        title.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chức vụ nhân viên");
        }

        return Updated(title);
    }

    [HttpDelete("EmployeeTitles({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var title = await _context.EmployeeTitles.FindAsync(key);
        if (title == null)
        {
            return NotFound();
        }

        var hasEmployees = await _context.Employees.AnyAsync(e => e.EmployeeTitleId == key);
        if (hasEmployees)
        {
            return BadRequest("Không thể xóa chức vụ đang được sử dụng bởi nhân viên.");
        }

        _context.EmployeeTitles.Remove(title);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chức vụ nhân viên");
        }

        return NoContent();
    }

    private bool EmployeeTitleExists(Guid key)
    {
        return _context.EmployeeTitles.Any(e => e.EmployeeTitleId == key);
    }
}
