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
public class ReferenceMethodsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReferenceMethodsController> _logger;

    public ReferenceMethodsController(ApplicationDbContext context, ILogger<ReferenceMethodsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("ReferenceMethods")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ReferenceMethods.Include(r => r.UpdatedByAccount));
    }

    [HttpGet("ReferenceMethods({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.ReferenceMethods
            .Include(r => r.UpdatedByAccount)
            .FirstOrDefault(r => r.ReferenceMethodId == key);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost("ReferenceMethods")]
    public async Task<IActionResult> Post([FromBody] ReferenceMethod referenceMethod)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        referenceMethod.ReferenceMethodId = referenceMethod.ReferenceMethodId == Guid.Empty ? Guid.NewGuid() : referenceMethod.ReferenceMethodId;
        referenceMethod.CreatedAt = DateTime.UtcNow;
        _context.ReferenceMethods.Add(referenceMethod);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu phương pháp tham chiếu");
        }

        return Created($"odata/ReferenceMethods({referenceMethod.ReferenceMethodId})", referenceMethod);
    }

    [HttpPut("ReferenceMethods({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ReferenceMethod referenceMethod)
    {
        if (key != referenceMethod.ReferenceMethodId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _context.ReferenceMethods.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        existing.SequenceNumber = referenceMethod.SequenceNumber;
        existing.NameVi = referenceMethod.NameVi;
        existing.NameEn = referenceMethod.NameEn;
        existing.ReferenceMethodCode = referenceMethod.ReferenceMethodCode;
        existing.Status = referenceMethod.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReferenceMethodExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật phương pháp tham chiếu");
        }

        await _context.Entry(existing).Reference(r => r.UpdatedByAccount).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("ReferenceMethods({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.ReferenceMethods.FindAsync(key);
        if (item == null)
        {
            return NotFound();
        }

        _context.ReferenceMethods.Remove(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa phương pháp tham chiếu");
        }

        return NoContent();
    }

    private bool ReferenceMethodExists(Guid key)
    {
        return _context.ReferenceMethods.Any(e => e.ReferenceMethodId == key);
    }
}
