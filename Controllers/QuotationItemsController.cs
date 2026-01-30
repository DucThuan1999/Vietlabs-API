using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class QuotationItemsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public QuotationItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("QuotationItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationItems
            .Include(qi => qi.Quotation)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package));
    }

    [HttpGet("QuotationItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var quotationItem = _context.QuotationItems
            .Include(qi => qi.Quotation)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package)
            .FirstOrDefault(qi => qi.QuotationItemId == key);
        if (quotationItem == null)
        {
            return NotFound();
        }
        return Ok(quotationItem);
    }

    [HttpPost("QuotationItems")]
    public async Task<IActionResult> Post([FromBody] QuotationItem quotationItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotationItem.QuotationItemId = quotationItem.QuotationItemId == Guid.Empty ? Guid.NewGuid() : quotationItem.QuotationItemId;
        quotationItem.CreatedAt = DateTime.UtcNow;
        _context.QuotationItems.Add(quotationItem);
        await _context.SaveChangesAsync();

        return Created($"odata/QuotationItems({quotationItem.QuotationItemId})", quotationItem);
    }

    [HttpPut("QuotationItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationItem quotationItem)
    {
        if (key != quotationItem.QuotationItemId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotationItem.UpdatedAt = DateTime.UtcNow;
        _context.Entry(quotationItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationItemExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(quotationItem);
    }

    [HttpDelete("QuotationItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var quotationItem = await _context.QuotationItems.FindAsync(key);
        if (quotationItem == null)
        {
            return NotFound();
        }

        _context.QuotationItems.Remove(quotationItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool QuotationItemExists(Guid key)
    {
        return _context.QuotationItems.Any(e => e.QuotationItemId == key);
    }
}

