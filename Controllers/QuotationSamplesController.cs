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
public class QuotationSamplesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationSamplesController> _logger;

    public QuotationSamplesController(ApplicationDbContext context, ILogger<QuotationSamplesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationSamples")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationSamples.Include(qs => qs.Quotation));
    }

    [HttpGet("QuotationSamples({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.QuotationSamples
            .Include(qs => qs.Quotation)
            .FirstOrDefault(qs => qs.QuotationSampleId == key);
        if (row == null)
        {
            return NotFound();
        }
        return Ok(row);
    }

    [HttpPost("QuotationSamples")]
    public async Task<IActionResult> Post([FromBody] System.Text.Json.JsonElement body)
    {
        QuotationSample? entity = null;

        try
        {
            if (body.TryGetProperty("quotationSample", out var wrapped))
            {
                entity = System.Text.Json.JsonSerializer.Deserialize<QuotationSample>(
                    wrapped.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
            else
            {
                entity = System.Text.Json.JsonSerializer.Deserialize<QuotationSample>(
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
            return BadRequest(new
            {
                error = "Invalid JSON format",
                message = $"Failed to parse request body: {ex.Message}"
            });
        }

        if (entity == null)
        {
            return BadRequest(new { error = "Body required", message = "Failed to deserialize QuotationSample." });
        }

        if (entity.QuotationId == Guid.Empty)
        {
            return BadRequest(new { error = "QuotationId required", message = "QuotationId must be set." });
        }

        entity.QuotationSampleId = entity.QuotationSampleId == Guid.Empty ? Guid.NewGuid() : entity.QuotationSampleId;
        entity.CreatedAt = DateTime.UtcNow;
        _context.QuotationSamples.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu dòng mẫu báo giá");
        }

        return Created($"odata/QuotationSamples({entity.QuotationSampleId})", entity);
    }

    [HttpPut("QuotationSamples({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationSample entity)
    {
        if (key != entity.QuotationSampleId)
        {
            return BadRequest();
        }

        if (entity.QuotationId == Guid.Empty)
        {
            return BadRequest(new { error = "QuotationId required", message = "QuotationId must be set." });
        }

        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Exists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật dòng mẫu báo giá");
        }

        return Updated(entity);
    }

    [HttpDelete("QuotationSamples({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.QuotationSamples.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.QuotationSamples.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa dòng mẫu báo giá");
        }

        return NoContent();
    }

    private bool Exists(Guid key) => _context.QuotationSamples.Any(e => e.QuotationSampleId == key);
}
