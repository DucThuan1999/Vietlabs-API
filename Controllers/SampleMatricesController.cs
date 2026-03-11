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
public class SampleMatricesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SampleMatricesController> _logger;

    public SampleMatricesController(ApplicationDbContext context, ILogger<SampleMatricesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("SampleMatrices")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.SampleMatrices
            .Include(sm => sm.UpdatedByAccount)
            .Include(sm => sm.SampleMatrixGroup));
    }

    [HttpGet("SampleMatrices({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var matrix = _context.SampleMatrices
            .Include(sm => sm.UpdatedByAccount)
            .Include(sm => sm.SampleMatrixGroup)
            .FirstOrDefault(sm => sm.SampleMatrixId == key);
        if (matrix == null)
        {
            return NotFound();
        }
        return Ok(matrix);
    }

    [HttpPost("SampleMatrices")]
    public async Task<IActionResult> Post([FromBody] SampleMatrix matrix)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        matrix.SampleMatrixId = matrix.SampleMatrixId == Guid.Empty ? Guid.NewGuid() : matrix.SampleMatrixId;
        matrix.CreatedAt = DateTime.UtcNow;
        _context.SampleMatrices.Add(matrix);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu mẫu vật");
        }

        return Created($"odata/SampleMatrices({matrix.SampleMatrixId})", matrix);
    }

    [HttpPut("SampleMatrices({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] SampleMatrix matrix)
    {
        if (key != matrix.SampleMatrixId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.SampleMatrices.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SampleMatrixCode = matrix.SampleMatrixCode;
        existing.NameVi = matrix.NameVi;
        existing.NameEn = matrix.NameEn;
        existing.SampleMatrixGroupId = matrix.SampleMatrixGroupId;
        existing.RegisteredMatrix = matrix.RegisteredMatrix;
        existing.Status = matrix.Status;
        existing.Notes = matrix.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SampleMatrixExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật mẫu vật");
        }

        await _context.Entry(existing).Reference(sm => sm.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("SampleMatrices({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var matrix = await _context.SampleMatrices.FindAsync(key);
        if (matrix == null)
        {
            return NotFound();
        }

        _context.SampleMatrices.Remove(matrix);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa mẫu vật");
        }

        return NoContent();
    }

    private bool SampleMatrixExists(Guid key)
    {
        return _context.SampleMatrices.Any(e => e.SampleMatrixId == key);
    }
}

