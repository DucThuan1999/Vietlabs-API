using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class QuotationsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public QuotationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Quotations")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Quotations
            .Include(q => q.Client)
            .Include(q => q.Employee)
            .Include(q => q.Contact)
            .Include(q => q.QuotationItems));
    }

    [HttpGet("Quotations({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var quotation = _context.Quotations
            .Include(q => q.Client)
            .Include(q => q.Employee)
            .Include(q => q.Contact)
            .Include(q => q.QuotationItems)
            .FirstOrDefault(q => q.QuotationId == key);
        if (quotation == null)
        {
            return NotFound();
        }
        return Ok(quotation);
    }

    [HttpPost("Quotations")]
    public async Task<IActionResult> Post([FromBody] Quotation quotation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotation.QuotationId = quotation.QuotationId == Guid.Empty ? Guid.NewGuid() : quotation.QuotationId;
        quotation.CreatedAt = DateTime.UtcNow;
        _context.Quotations.Add(quotation);
        await _context.SaveChangesAsync();

        return Created($"odata/Quotations({quotation.QuotationId})", quotation);
    }

    [HttpPut("Quotations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Quotation quotation)
    {
        if (key != quotation.QuotationId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotation.UpdatedAt = DateTime.UtcNow;
        _context.Entry(quotation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(quotation);
    }

    [HttpDelete("Quotations({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var quotation = await _context.Quotations.FindAsync(key);
        if (quotation == null)
        {
            return NotFound();
        }

        _context.Quotations.Remove(quotation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool QuotationExists(Guid key)
    {
        return _context.Quotations.Any(e => e.QuotationId == key);
    }
}

