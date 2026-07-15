using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Helpers;
using VietLab.Models;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class ProvincesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProvincesController> _logger;

    public ProvincesController(
        ApplicationDbContext context,
        ILogger<ProvincesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Provinces")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Provinces
            .Include(p => p.Country)
            .Include(p => p.Wards));
    }
    
    // Endpoint tiện ích: Lấy tất cả provinces theo countryId
    [HttpGet("Provinces/ByCountry({countryId})")]
    [EnableQuery]
    public IActionResult GetByCountry([FromRoute] Guid countryId)
    {
        var provinces = _context.Provinces
            .Where(p => p.CountryId == countryId)
            .AsNoTracking();
        
        return Ok(provinces);
    }

    [HttpGet("Provinces({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var province = _context.Provinces
            .Include(p => p.Country)
            .Include(p => p.Wards)
            .FirstOrDefault(p => p.ProvinceId == key);

        if (province == null)
        {
            return NotFound();
        }

        return Ok(province);
    }

    [HttpPost("Provinces")]
    public async Task<IActionResult> Post([FromBody] Province province)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate CountryId exists
        var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == province.CountryId);
        if (!countryExists)
        {
            return BadRequest($"Country with ID {province.CountryId} does not exist.");
        }

        province.ProvinceId = province.ProvinceId == Guid.Empty ? Guid.NewGuid() : province.ProvinceId;
        if (string.IsNullOrEmpty(province.Status))
        {
            province.Status = "Active";
        }

        _context.Provinces.Add(province);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu tỉnh/thành phố");
        }

        return Created($"odata/Provinces({province.ProvinceId})", province);
    }

    [HttpPut("Provinces({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Province province)
    {
        if (key != province.ProvinceId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate CountryId exists
        var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == province.CountryId);
        if (!countryExists)
        {
            return BadRequest($"Country with ID {province.CountryId} does not exist.");
        }

        _context.Entry(province).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProvinceExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật tỉnh/thành phố");
        }

        return Updated(province);
    }

    [HttpDelete("Provinces({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var province = await _context.Provinces.FindAsync(key);
        if (province == null)
        {
            return NotFound();
        }

        // Check if province has wards
        var hasWards = await _context.Wards.AnyAsync(w => w.ProvinceId == key);
        if (hasWards)
        {
            return BadRequest("Cannot delete province that has wards.");
        }

        _context.Provinces.Remove(province);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa tỉnh/thành phố");
        }

        return NoContent();
    }

    private bool ProvinceExists(Guid key)
    {
        return _context.Provinces.Any(e => e.ProvinceId == key);
    }
}

