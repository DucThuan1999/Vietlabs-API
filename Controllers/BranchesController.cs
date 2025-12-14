using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class BranchesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public BranchesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Branches")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Branches);
    }

    [HttpGet("Branches({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var branch = _context.Branches.FirstOrDefault(b => b.BranchId == key);
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
        await _context.SaveChangesAsync();

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

        return Updated(branch);
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
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BranchExists(Guid key)
    {
        return _context.Branches.Any(e => e.BranchId == key);
    }
}


