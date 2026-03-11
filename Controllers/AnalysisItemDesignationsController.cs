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
public class AnalysisItemDesignationsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnalysisItemDesignationsController> _logger;

    public AnalysisItemDesignationsController(
        ApplicationDbContext context,
        ILogger<AnalysisItemDesignationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("AnalysisItemDesignations")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.AnalysisItemDesignations
            .Include(aid => aid.AnalysisItem)
            .Include(aid => aid.Designation));
    }

    [HttpGet("AnalysisItemDesignations({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var item = _context.AnalysisItemDesignations
            .Include(aid => aid.AnalysisItem)
            .Include(aid => aid.Designation)
            .FirstOrDefault(aid => aid.AnalysisItemDesignationId == key);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost("AnalysisItemDesignations")]
    public async Task<IActionResult> Post([FromBody] AnalysisItemDesignation item)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        item.AnalysisItemDesignationId = item.AnalysisItemDesignationId == Guid.Empty
            ? Guid.NewGuid()
            : item.AnalysisItemDesignationId;
        _context.AnalysisItemDesignations.Add(item);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ định chỉ tiêu");
        }

        return Created($"odata/AnalysisItemDesignations({item.AnalysisItemDesignationId})", item);
    }

    [HttpPut("AnalysisItemDesignations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] AnalysisItemDesignation item)
    {
        if (key != item.AnalysisItemDesignationId)
            return BadRequest();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.AnalysisItemDesignations.FindAsync(key);
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
            return this.HandleDatabaseError(ex, _logger, "cập nhật chỉ định chỉ tiêu");
        }

        await _context.Entry(existing).Reference(aid => aid.Designation).LoadAsync();
        return Updated(existing);
    }

    [HttpDelete("AnalysisItemDesignations({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var item = await _context.AnalysisItemDesignations.FindAsync(key);
        if (item == null)
            return NotFound();

        _context.AnalysisItemDesignations.Remove(item);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ định chỉ tiêu");
        }
        return NoContent();
    }
}

