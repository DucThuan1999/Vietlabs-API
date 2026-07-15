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
public class RegistrationPermitLabelConfigsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegistrationPermitLabelConfigsController> _logger;

    public RegistrationPermitLabelConfigsController(
        ApplicationDbContext context,
        ILogger<RegistrationPermitLabelConfigsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Bản ghi singleton cấu hình tên hiển thị giấy phép đăng ký.
    /// </summary>
    [HttpGet("RegistrationPermitLabelConfigs/Current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
    {
        var row = await _context.RegistrationPermitLabelConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.RegistrationPermitLabelConfigId == RegistrationPermitLabelReader.SingletonId,
                cancellationToken);

        if (row == null)
        {
            return Ok(new
            {
                RegistrationPermitLabelConfigId = RegistrationPermitLabelReader.SingletonId,
                DisplayName = RegistrationPermitLabelReader.DefaultDisplayName,
            });
        }

        return Ok(new
        {
            row.RegistrationPermitLabelConfigId,
            DisplayName = RegistrationPermitLabelReader.NormalizeDisplayName(row.DisplayName),
            row.CreatedAt,
            row.UpdatedAt,
            row.UpdatedBy,
        });
    }

    [HttpGet("RegistrationPermitLabelConfigs")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.RegistrationPermitLabelConfigs);
    }

    [HttpGet("RegistrationPermitLabelConfigs({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var row = _context.RegistrationPermitLabelConfigs
            .FirstOrDefault(t => t.RegistrationPermitLabelConfigId == key);
        if (row == null)
        {
            return NotFound();
        }
        return Ok(row);
    }

    [HttpPut("RegistrationPermitLabelConfigs({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] RegistrationPermitLabelConfig entity)
    {
        if (key != entity.RegistrationPermitLabelConfigId)
        {
            return BadRequest(new
            {
                error = "Key mismatch",
                message = $"The key in URL ({key}) does not match RegistrationPermitLabelConfigId in body ({entity.RegistrationPermitLabelConfigId})"
            });
        }

        var validation = ValidateDisplayName(entity.DisplayName);
        if (validation != null)
        {
            return BadRequest(new { error = "Validation failed", message = validation });
        }

        var existing = await _context.RegistrationPermitLabelConfigs.FindAsync(key);
        if (existing == null)
        {
            entity.DisplayName = entity.DisplayName.Trim();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _context.RegistrationPermitLabelConfigs.Add(entity);
        }
        else
        {
            existing.DisplayName = entity.DisplayName.Trim();
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = entity.UpdatedBy;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update registration permit label config");
            return StatusCode(500, new { error = "Database error", message = ex.Message });
        }

        var saved = await _context.RegistrationPermitLabelConfigs
            .AsNoTracking()
            .FirstAsync(x => x.RegistrationPermitLabelConfigId == key);

        return Ok(saved);
    }

    private static string? ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return "DisplayName is required.";
        }
        if (displayName.Trim().Length > 200)
        {
            return "DisplayName must not exceed 200 characters.";
        }
        return null;
    }
}
