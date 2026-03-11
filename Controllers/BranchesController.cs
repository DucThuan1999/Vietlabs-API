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
public class BranchesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BranchesController> _logger;

    public BranchesController(ApplicationDbContext context, ILogger<BranchesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("Branches")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Branches.Include(b => b.UpdatedByAccount));
    }

    [HttpGet("Branches({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var branch = _context.Branches.Include(b => b.UpdatedByAccount).FirstOrDefault(b => b.BranchId == key);
        if (branch == null)
        {
            return NotFound();
        }
        return Ok(branch);
    }

    [HttpPost("Branches")]
    public async Task<IActionResult> Post([FromBody] Branch branch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        branch.BranchId = branch.BranchId == Guid.Empty ? Guid.NewGuid() : branch.BranchId;
        _context.Branches.Add(branch);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chi nhánh");
        }

        return Created($"odata/Branches({branch.BranchId})", branch);
    }

    [HttpPut("Branches({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Branch branch)
    {
        if (key != branch.BranchId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        branch.UpdatedAt = DateTime.UtcNow;
        branch.UpdatedBy = GetCurrentAccountId();
        _context.Entry(branch).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BranchExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chi nhánh");
        }

        var updated = await _context.Branches.Include(b => b.UpdatedByAccount).FirstOrDefaultAsync(b => b.BranchId == key);
        return Updated(updated ?? branch);
    }

    [HttpDelete("Branches({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var branch = await _context.Branches.FindAsync(key);
        if (branch == null)
        {
            return NotFound();
        }

        _context.Branches.Remove(branch);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chi nhánh");
        }

        return NoContent();
    }

    private bool BranchExists(Guid key)
    {
        return _context.Branches.Any(e => e.BranchId == key);
    }
}


