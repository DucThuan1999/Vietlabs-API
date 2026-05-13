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
public class VatRatesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VatRatesController> _logger;

    public VatRatesController(ApplicationDbContext context, ILogger<VatRatesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// VAT hợp lệ tại ngày asOf: Status Active, StartDate &lt;= ngày, (EndDate null hoặc EndDate &gt;= ngày).
    /// Trùng khoảng: lấy StartDate mới nhất, tie-break CreatedAt mới nhất.
    /// </summary>
    [HttpGet("VatRates/Effective")]
    public async Task<IActionResult> GetEffective([FromQuery] DateTime? asOf = null, CancellationToken cancellationToken = default)
    {
        var dayStart = DateTime.SpecifyKind((asOf ?? DateTime.UtcNow).Date, DateTimeKind.Utc);
        var dayEndExclusive = dayStart.AddDays(1);

        var rate = await _context.VatRates
            .AsNoTracking()
            .Where(v => v.Status == "Active")
            .Where(v => v.StartDate < dayEndExclusive && (v.EndDate == null || v.EndDate >= dayStart))
            .OrderByDescending(v => v.StartDate)
            .ThenByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (rate == null)
        {
            return NotFound(new { message = "Không có mức VAT hợp lệ cho ngày đã chọn." });
        }

        return Ok(new
        {
            rate.VatRateId,
            rate.Percent,
            rate.StartDate,
            rate.EndDate,
            rate.Description,
            rate.Status,
            AsOfDate = dayStart,
        });
    }

    [HttpGet("VatRates")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.VatRates);
    }

    [HttpGet("VatRates({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.VatRates
            .FirstOrDefault(t => t.VatRateId == key);
        if (row == null)
        {
            return NotFound();
        }
        return Ok(row);
    }

    [HttpPost("VatRates")]
    public async Task<IActionResult> Post([FromBody] VatRate entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var validation = ValidateVatRate(entity);
        if (validation != null)
        {
            return BadRequest(new { error = validation });
        }

        entity.VatRateId = entity.VatRateId == Guid.Empty
            ? Guid.NewGuid()
            : entity.VatRateId;
        entity.CreatedAt = DateTime.UtcNow;

        _context.VatRates.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu mức VAT");
        }

        return Created($"odata/VatRates({entity.VatRateId})", entity);
    }

    [HttpPut("VatRates({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] VatRate entity)
    {
        if (key != entity.VatRateId)
        {
            return BadRequest(new
            {
                error = "Key mismatch",
                message = $"The key in URL ({key}) does not match VatRateId in body ({entity.VatRateId})"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var validation = ValidateVatRate(entity);
        if (validation != null)
        {
            return BadRequest(new { error = validation });
        }

        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VatRateExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật mức VAT");
        }

        return Updated(entity);
    }

    [HttpDelete("VatRates({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.VatRates.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.VatRates.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa mức VAT");
        }

        return NoContent();
    }

    private static string? ValidateVatRate(VatRate entity)
    {
        if (entity.Percent < 0)
        {
            return "Phần trăm VAT phải >= 0.";
        }

        if (entity.EndDate.HasValue && entity.EndDate.Value.Date < entity.StartDate.Date)
        {
            return "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.";
        }

        return null;
    }

    private bool VatRateExists(Guid key)
    {
        return _context.VatRates.Any(e => e.VatRateId == key);
    }
}
