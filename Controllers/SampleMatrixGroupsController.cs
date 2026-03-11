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
public class SampleMatrixGroupsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SampleMatrixGroupsController> _logger;

    public SampleMatrixGroupsController(ApplicationDbContext context, ILogger<SampleMatrixGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("SampleMatrixGroups")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.SampleMatrixGroups
            .Include(smg => smg.UpdatedByAccount)
            .Include(smg => smg.SampleMatrices));
    }

    [HttpGet("SampleMatrixGroups({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var group = _context.SampleMatrixGroups
            .Include(smg => smg.UpdatedByAccount)
            .Include(smg => smg.SampleMatrices)
            .FirstOrDefault(smg => smg.SampleMatrixGroupId == key);
        if (group == null)
        {
            return NotFound();
        }
        return Ok(group);
    }

    [HttpPost("SampleMatrixGroups")]
    public async Task<IActionResult> Post([FromBody] SampleMatrixGroup group)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        group.SampleMatrixGroupId = group.SampleMatrixGroupId == Guid.Empty ? Guid.NewGuid() : group.SampleMatrixGroupId;
        group.CreatedAt = DateTime.UtcNow;
        _context.SampleMatrixGroups.Add(group);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu nhóm mẫu vật");
        }

        return Created($"odata/SampleMatrixGroups({group.SampleMatrixGroupId})", group);
    }

    [HttpPut("SampleMatrixGroups({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] SampleMatrixGroup group)
    {
        if (key != group.SampleMatrixGroupId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.SampleMatrixGroups.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SampleMatrixGroupCode = group.SampleMatrixGroupCode;
        existing.NameVi = group.NameVi;
        existing.NameEn = group.NameEn;
        existing.Status = group.Status;
        existing.Notes = group.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SampleMatrixGroupExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật nhóm mẫu vật");
        }

        await _context.Entry(existing).Reference(smg => smg.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("SampleMatrixGroups({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var group = await _context.SampleMatrixGroups.FindAsync(key);
        if (group == null)
        {
            return NotFound();
        }

        _context.SampleMatrixGroups.Remove(group);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa nhóm mẫu vật");
        }

        return NoContent();
    }

    private bool SampleMatrixGroupExists(Guid key)
    {
        return _context.SampleMatrixGroups.Any(e => e.SampleMatrixGroupId == key);
    }
}

