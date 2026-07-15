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
public class QuotationSurchargesController : ODataController
{
    private static readonly HashSet<string> AllowedSurchargeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Transportation",
        "PrintResult",
        "SamplingLabor",
        "SamplingTools",
        "Other",
    };

    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationSurchargesController> _logger;

    public QuotationSurchargesController(ApplicationDbContext context, ILogger<QuotationSurchargesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationSurcharges")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationSurcharges
            .Include(x => x.Quotation)
            .Include(x => x.UpdatedByAccount));
    }

    [HttpGet("QuotationSurcharges({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var entity = _context.QuotationSurcharges
            .Include(x => x.Quotation)
            .Include(x => x.UpdatedByAccount)
            .FirstOrDefault(x => x.QuotationSurchargeId == key);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpPost("QuotationSurcharges")]
    public async Task<IActionResult> Post([FromBody] System.Text.Json.JsonElement body)
    {
        QuotationSurcharge? entity = DeserializeEntity(body);
        if (entity == null)
        {
            return BadRequest(new { error = "Body required", message = "Failed to deserialize QuotationSurcharge." });
        }

        var validationError = ValidateEntity(entity);
        if (validationError != null)
            return BadRequest(new { error = validationError });

        entity.QuotationSurchargeId = entity.QuotationSurchargeId == Guid.Empty
            ? Guid.NewGuid()
            : entity.QuotationSurchargeId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.SurchargeType = entity.SurchargeType.Trim();

        _context.QuotationSurcharges.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu phụ phí báo giá");
        }

        return Created($"odata/QuotationSurcharges({entity.QuotationSurchargeId})", entity);
    }

    [HttpPut("QuotationSurcharges({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] System.Text.Json.JsonElement body)
    {
        var existing = await _context.QuotationSurcharges.FindAsync(key);
        if (existing == null)
            return NotFound();

        QuotationSurcharge? entity = DeserializeEntity(body);
        if (entity == null)
        {
            return BadRequest(new { error = "Body required", message = "Failed to deserialize QuotationSurcharge." });
        }

        entity.QuotationSurchargeId = key;
        var validationError = ValidateEntity(entity);
        if (validationError != null)
            return BadRequest(new { error = validationError });

        existing.QuotationId = entity.QuotationId;
        existing.SurchargeType = entity.SurchargeType.Trim();
        existing.Description = entity.Description;
        existing.Amount = entity.Amount;
        existing.DisplayOrder = entity.DisplayOrder;
        existing.Notes = entity.Notes;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = entity.UpdatedBy;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật phụ phí báo giá");
        }

        return Ok(existing);
    }

    [HttpDelete("QuotationSurcharges({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var entity = await _context.QuotationSurcharges.FindAsync(key);
        if (entity == null)
            return NotFound();

        _context.QuotationSurcharges.Remove(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa phụ phí báo giá");
        }

        return NoContent();
    }

    private static QuotationSurcharge? DeserializeEntity(System.Text.Json.JsonElement body)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };

        try
        {
            if (body.TryGetProperty("quotationSurcharge", out var wrapped))
            {
                return System.Text.Json.JsonSerializer.Deserialize<QuotationSurcharge>(wrapped.GetRawText(), options);
            }

            return System.Text.Json.JsonSerializer.Deserialize<QuotationSurcharge>(body.GetRawText(), options);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    private static string? ValidateEntity(QuotationSurcharge entity)
    {
        if (entity.QuotationId == Guid.Empty)
            return "QuotationId is required";

        if (string.IsNullOrWhiteSpace(entity.SurchargeType))
            return "SurchargeType is required";

        if (!AllowedSurchargeTypes.Contains(entity.SurchargeType.Trim()))
            return "Invalid SurchargeType";

        if (entity.Amount < 0)
            return "Amount must be >= 0";

        if (string.Equals(entity.SurchargeType.Trim(), "Other", StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(entity.Description))
        {
            return "Description is required when SurchargeType is Other";
        }

        return null;
    }
}
