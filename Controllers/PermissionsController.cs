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
public class PermissionsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(ApplicationDbContext context, ILogger<PermissionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Permissions")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Permissions);
    }

    [HttpGet("Permissions({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var permission = _context.Permissions.FirstOrDefault(p => p.PermissionId == key);
        if (permission == null)
        {
            return NotFound();
        }
        return Ok(permission);
    }

    [HttpPost("Permissions")]
    public async Task<IActionResult> Post([FromBody] Permission permission)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        permission.PermissionId = permission.PermissionId == Guid.Empty ? Guid.NewGuid() : permission.PermissionId;
        _context.Permissions.Add(permission);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu quyền");
        }

        return Created($"odata/Permissions({permission.PermissionId})", permission);
    }

    [HttpPut("Permissions({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Permission permission)
    {
        if (key != permission.PermissionId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(permission).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PermissionExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật quyền");
        }

        return Updated(permission);
    }

    [HttpDelete("Permissions({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var permission = await _context.Permissions.FindAsync(key);
        if (permission == null)
        {
            return NotFound();
        }

        _context.Permissions.Remove(permission);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa quyền");
        }

        return NoContent();
    }

    private bool PermissionExists(Guid key)
    {
        return _context.Permissions.Any(p => p.PermissionId == key);
    }
}


