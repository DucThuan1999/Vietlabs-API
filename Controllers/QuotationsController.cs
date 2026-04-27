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
public class QuotationsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationsController> _logger;

    public QuotationsController(ApplicationDbContext context, ILogger<QuotationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Quotations")]
    [EnableQuery]
    public IActionResult Get()
    {
        try
        {
            var query = _context.Quotations
                .AsNoTracking()
                .Include(q => q.Client)
                .Include(q => q.Employee)
                .Include(q => q.Contact)
                .Include(q => q.QuotationItems)
                .Include(q => q.QuotationSamples)
                .Include(q => q.QuotationAnalysisGroups);

            var result = query.ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while querying Quotations");
            throw;
        }
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
            .Include(q => q.QuotationSamples)
            .Include(q => q.QuotationAnalysisGroups)
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu báo giá");
        }

        return Created($"odata/Quotations({quotation.QuotationId})", quotation);
    }

    [HttpPut("Quotations({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] System.Text.Json.JsonElement body)
    {
        Quotation? quotation = null;
        
        try
        {
            // Nếu có field "quotationDataToSave", lấy từ đó (frontend wrapper format)
            if (body.TryGetProperty("quotationDataToSave", out var quotationDataElement))
            {
                quotation = System.Text.Json.JsonSerializer.Deserialize<Quotation>(
                    quotationDataElement.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
            // Nếu không có wrapper, deserialize trực tiếp (OData standard format)
            else
            {
                quotation = System.Text.Json.JsonSerializer.Deserialize<Quotation>(
                    body.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
        }
        catch (System.Text.Json.JsonException ex)
        {
            return BadRequest(new { 
                error = "Invalid JSON format",
                message = $"Failed to parse request body: {ex.Message}"
            });
        }

        if (quotation == null)
        {
            return BadRequest(new { 
                error = "A non-empty request body is required.",
                message = "The quotation field is required."
            });
        }

        if (key != quotation.QuotationId)
        {
            return BadRequest(new { 
                error = "Key mismatch",
                message = $"The key in URL ({key}) does not match QuotationId in body ({quotation.QuotationId})"
            });
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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật báo giá");
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa báo giá");
        }

        return NoContent();
    }

    private bool QuotationExists(Guid key)
    {
        return _context.Quotations.Any(e => e.QuotationId == key);
    }
}

