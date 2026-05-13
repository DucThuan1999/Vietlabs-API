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
public class QuotationNonNd107ItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationNonNd107ItemsController> _logger;

    public QuotationNonNd107ItemsController(ApplicationDbContext context, ILogger<QuotationNonNd107ItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationNonNd107Items")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationNonNd107Items
            .Include(x => x.Quotation)
            .Include(x => x.AnalysisItem)
            .Include(x => x.AnalysisGroup)
            .Include(x => x.Package));
    }

    [HttpGet("QuotationNonNd107Items({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var entity = _context.QuotationNonNd107Items
            .Include(x => x.Quotation)
            .Include(x => x.AnalysisItem)
            .Include(x => x.AnalysisGroup)
            .Include(x => x.Package)
            .FirstOrDefault(x => x.QuotationNonNd107ItemId == key);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpPost("QuotationNonNd107Items")]
    public async Task<IActionResult> Post([FromBody] System.Text.Json.JsonElement body)
    {
        QuotationNonNd107Item? entity = null;

        try
        {
            if (body.TryGetProperty("quotationNonNd107Item", out var wrapped))
            {
                entity = System.Text.Json.JsonSerializer.Deserialize<QuotationNonNd107Item>(
                    wrapped.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
            else
            {
                entity = System.Text.Json.JsonSerializer.Deserialize<QuotationNonNd107Item>(
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
            return BadRequest(new { error = "Invalid JSON format", message = ex.Message });
        }

        if (entity == null)
        {
            return BadRequest(new { error = "Body required", message = "Failed to deserialize QuotationNonNd107Item." });
        }

        if (entity.QuotationId == Guid.Empty)
        {
            return BadRequest(new { error = "QuotationId is required" });
        }

        if (string.IsNullOrWhiteSpace(entity.SourceType))
        {
            return BadRequest(new { error = "SourceType is required" });
        }

        entity.QuotationNonNd107ItemId = entity.QuotationNonNd107ItemId == Guid.Empty
            ? Guid.NewGuid()
            : entity.QuotationNonNd107ItemId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.SourceType = entity.SourceType.Trim();

        _context.QuotationNonNd107Items.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu chỉ tiêu chưa NĐ107");
        }

        return Created($"odata/QuotationNonNd107Items({entity.QuotationNonNd107ItemId})", entity);
    }

    [HttpDelete("QuotationNonNd107Items({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var entity = await _context.QuotationNonNd107Items.FindAsync(key);
        if (entity == null)
            return NotFound();

        _context.QuotationNonNd107Items.Remove(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa chỉ tiêu chưa NĐ107");
        }

        return NoContent();
    }
}
