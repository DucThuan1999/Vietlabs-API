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
public class ClientIndustriesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClientIndustriesController> _logger;

    public ClientIndustriesController(
        ApplicationDbContext context,
        ILogger<ClientIndustriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        return Guid.TryParse(accountIdClaim, out var accountId) ? accountId : null;
    }

    [HttpGet("ClientIndustries")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ClientIndustries
            .Include(c => c.CreatedByAccount)
            .Include(c => c.UpdatedByAccount));
    }

    [HttpGet("ClientIndustries({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var industry = _context.ClientIndustries
            .Include(c => c.CreatedByAccount)
            .Include(c => c.UpdatedByAccount)
            .FirstOrDefault(c => c.ClientIndustryId == key);
        if (industry == null)
        {
            return NotFound();
        }
        return Ok(industry);
    }

    [HttpPost("ClientIndustries")]
    public async Task<IActionResult> Post([FromBody] ClientIndustry industry)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        industry.ClientIndustryId = industry.ClientIndustryId == Guid.Empty ? Guid.NewGuid() : industry.ClientIndustryId;
        if (string.IsNullOrEmpty(industry.Status))
        {
            industry.Status = "Active";
        }
        industry.CreatedAt = DateTime.UtcNow;
        industry.CreatedBy = GetCurrentAccountId();

        _context.ClientIndustries.Add(industry);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu ngành nghề khách hàng");
        }

        return Created($"odata/ClientIndustries({industry.ClientIndustryId})", industry);
    }

    [HttpPut("ClientIndustries({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ClientIndustry industry)
    {
        if (key != industry.ClientIndustryId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        industry.UpdatedAt = DateTime.UtcNow;
        industry.UpdatedBy = GetCurrentAccountId();
        _context.Entry(industry).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientIndustryExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật ngành nghề khách hàng");
        }

        await _context.Entry(industry).Reference(c => c.UpdatedByAccount).LoadAsync();
        return Updated(industry);
    }

    [HttpPatch("ClientIndustries({key})")]
    public async Task<IActionResult> Patch([FromRoute] Guid key, [FromBody] Microsoft.AspNetCore.OData.Deltas.Delta<ClientIndustry> patch)
    {
        var industry = await _context.ClientIndustries.FindAsync(key);
        if (industry == null)
        {
            return NotFound();
        }

        patch.Patch(industry);
        industry.UpdatedAt = DateTime.UtcNow;
        industry.UpdatedBy = GetCurrentAccountId();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật ngành nghề khách hàng");
        }

        await _context.Entry(industry).Reference(c => c.UpdatedByAccount).LoadAsync();
        return Updated(industry);
    }

    [HttpDelete("ClientIndustries({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var industry = await _context.ClientIndustries.FindAsync(key);
        if (industry == null)
        {
            return NotFound();
        }

        var hasClients = await _context.Clients.AnyAsync(c => c.ClientIndustryId == key);
        if (hasClients)
        {
            return BadRequest("Không thể xóa ngành nghề đang được sử dụng bởi khách hàng.");
        }

        _context.ClientIndustries.Remove(industry);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa ngành nghề khách hàng");
        }

        return NoContent();
    }

    private bool ClientIndustryExists(Guid key)
    {
        return _context.ClientIndustries.Any(e => e.ClientIndustryId == key);
    }
}
