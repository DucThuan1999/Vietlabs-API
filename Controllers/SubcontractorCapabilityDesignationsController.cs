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
public class SubcontractorCapabilityDesignationsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubcontractorCapabilityDesignationsController> _logger;

    public SubcontractorCapabilityDesignationsController(
        ApplicationDbContext context,
        ILogger<SubcontractorCapabilityDesignationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("SubcontractorCapabilityDesignations")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.SubcontractorCapabilityDesignations
            .Include(scd => scd.Designation));
    }

    [HttpGet("SubcontractorCapabilityDesignations({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.SubcontractorCapabilityDesignations
            .Include(scd => scd.SubcontractorCapability)
            .Include(scd => scd.Designation)
            .FirstOrDefault(scd => scd.SubcontractorCapabilityDesignationId == key);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost("SubcontractorCapabilityDesignations")]
    public async Task<IActionResult> Post([FromBody] SubcontractorCapabilityDesignation item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        item.SubcontractorCapabilityDesignationId = item.SubcontractorCapabilityDesignationId == Guid.Empty
            ? Guid.NewGuid()
            : item.SubcontractorCapabilityDesignationId;
        _context.SubcontractorCapabilityDesignations.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ định năng lực nhà thầu phụ");
        }

        return Created($"odata/SubcontractorCapabilityDesignations({item.SubcontractorCapabilityDesignationId})", item);
    }

    [HttpPut("SubcontractorCapabilityDesignations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] SubcontractorCapabilityDesignation item)
    {
        if (key != item.SubcontractorCapabilityDesignationId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.SubcontractorCapabilityDesignations.FindAsync(key);
        if (existing == null)
            return NotFound();

        existing.DesignationId = item.DesignationId;
        existing.ExpiredDate = item.ExpiredDate;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ định năng lực nhà thầu phụ");
        }

        await _context.Entry(existing).Reference(scd => scd.Designation).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("SubcontractorCapabilityDesignations({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.SubcontractorCapabilityDesignations.FindAsync(key);
        if (item == null)
            return NotFound();

        _context.SubcontractorCapabilityDesignations.Remove(item);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ định năng lực nhà thầu phụ");
        }
        return NoContent();
    }
}
