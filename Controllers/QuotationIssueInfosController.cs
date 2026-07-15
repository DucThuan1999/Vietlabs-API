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
public class QuotationIssueInfosController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationIssueInfosController> _logger;

    public QuotationIssueInfosController(ApplicationDbContext context, ILogger<QuotationIssueInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Bản ghi hợp lệ tại ngày asOf: Status Active, StartDate &lt;= ngày, (EndDate null hoặc EndDate &gt;= ngày).
    /// Trùng khoảng: lấy StartDate mới nhất, tie-break CreatedAt mới nhất.
    /// </summary>
    [HttpGet("QuotationIssueInfos/Effective")]
    public async Task<IActionResult> GetEffective([FromQuery] DateTime? asOf = null, CancellationToken cancellationToken = default)
    {
        var dayStart = DateTime.SpecifyKind((asOf ?? DateTime.UtcNow).Date, DateTimeKind.Utc);
        var dayEndExclusive = dayStart.AddDays(1);

        var row = await _context.QuotationIssueInfos
            .AsNoTracking()
            .Where(v => v.Status == "Active")
            .Where(v => v.StartDate < dayEndExclusive && (v.EndDate == null || v.EndDate >= dayStart))
            .OrderByDescending(v => v.StartDate)
            .ThenByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (row == null)
        {
            return NotFound(new { message = "Không có thông tin ban hành hợp lệ cho ngày đã chọn." });
        }

        return Ok(new
        {
            row.QuotationIssueInfoId,
            row.Content,
            row.StartDate,
            row.EndDate,
            row.Description,
            row.Status,
            AsOfDate = dayStart,
        });
    }

    [HttpGet("QuotationIssueInfos")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationIssueInfos);
    }

    [HttpGet("QuotationIssueInfos({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.QuotationIssueInfos
            .FirstOrDefault(t => t.QuotationIssueInfoId == key);
        if (row == null)
        {
            return NotFound();
        }
        return Ok(row);
    }

    [HttpPost("QuotationIssueInfos")]
    public async Task<IActionResult> Post([FromBody] QuotationIssueInfo entity)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var validation = ValidateQuotationIssueInfo(entity);
        if (validation != null)
        {
            return BadRequest(new { error = validation });
        }

        entity.QuotationIssueInfoId = entity.QuotationIssueInfoId == Guid.Empty
            ? Guid.NewGuid()
            : entity.QuotationIssueInfoId;
        entity.CreatedAt = DateTime.UtcNow;

        _context.QuotationIssueInfos.Add(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu thông tin ban hành");
        }

        return Created($"odata/QuotationIssueInfos({entity.QuotationIssueInfoId})", entity);
    }

    [HttpPut("QuotationIssueInfos({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationIssueInfo entity)
    {
        if (key != entity.QuotationIssueInfoId)
        {
            return BadRequest(new
            {
                error = "Key mismatch",
                message = $"The key in URL ({key}) does not match QuotationIssueInfoId in body ({entity.QuotationIssueInfoId})"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var validation = ValidateQuotationIssueInfo(entity);
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
            if (!Exists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật thông tin ban hành");
        }

        return Updated(entity);
    }

    [HttpDelete("QuotationIssueInfos({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var row = await _context.QuotationIssueInfos.FindAsync(key);
        if (row == null)
        {
            return NotFound();
        }

        _context.QuotationIssueInfos.Remove(row);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa thông tin ban hành");
        }

        return NoContent();
    }

    private static string? ValidateQuotationIssueInfo(QuotationIssueInfo entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Content))
        {
            return "Nội dung thông tin ban hành không được để trống.";
        }

        if (entity.EndDate.HasValue && entity.EndDate.Value.Date < entity.StartDate.Date)
        {
            return "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.";
        }

        return null;
    }

    private bool Exists(Guid key)
    {
        return _context.QuotationIssueInfos.Any(e => e.QuotationIssueInfoId == key);
    }
}
