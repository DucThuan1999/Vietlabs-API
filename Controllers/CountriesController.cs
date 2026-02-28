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
public class CountriesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(
        ApplicationDbContext context,
        ILogger<CountriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Countries")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Countries
            .Include(c => c.Provinces));
    }

    [HttpGet("Countries({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var country = _context.Countries
            .Include(c => c.Provinces)
            .FirstOrDefault(c => c.CountryId == key);

        if (country == null)
        {
            return NotFound();
        }

        return Ok(country);
    }

    [HttpPost("Countries")]
    public async Task<IActionResult> Post([FromBody] Country country)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate Alpha2 length (must be exactly 2 characters if provided)
        if (!string.IsNullOrEmpty(country.Alpha2) && country.Alpha2.Length > 2)
        {
            ModelState.AddModelError(nameof(country.Alpha2), "Alpha2 code must be exactly 2 characters.");
            return BadRequest(ModelState);
        }

        // Validate Alpha3 length (must be exactly 3 characters if provided)
        if (!string.IsNullOrEmpty(country.Alpha3) && country.Alpha3.Length > 3)
        {
            ModelState.AddModelError(nameof(country.Alpha3), "Alpha3 code must be exactly 3 characters.");
            return BadRequest(ModelState);
        }

        country.CountryId = country.CountryId == Guid.Empty ? Guid.NewGuid() : country.CountryId;
        if (string.IsNullOrEmpty(country.Status))
        {
            country.Status = "Active";
        }

        _context.Countries.Add(country);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu quốc gia");
        }

        return Created($"odata/Countries({country.CountryId})", country);
    }

    [HttpPut("Countries({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Country country)
    {
        if (key != country.CountryId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate Alpha2 length (must be exactly 2 characters if provided)
        if (!string.IsNullOrEmpty(country.Alpha2) && country.Alpha2.Length > 2)
        {
            ModelState.AddModelError(nameof(country.Alpha2), "Alpha2 code must be exactly 2 characters.");
            return BadRequest(ModelState);
        }

        // Validate Alpha3 length (must be exactly 3 characters if provided)
        if (!string.IsNullOrEmpty(country.Alpha3) && country.Alpha3.Length > 3)
        {
            ModelState.AddModelError(nameof(country.Alpha3), "Alpha3 code must be exactly 3 characters.");
            return BadRequest(ModelState);
        }

        _context.Entry(country).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CountryExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật quốc gia");
        }

        return Updated(country);
    }

    [HttpDelete("Countries({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var country = await _context.Countries.FindAsync(key);
        if (country == null)
        {
            return NotFound();
        }

        // Check if country has provinces
        var hasProvinces = await _context.Provinces.AnyAsync(p => p.CountryId == key);
        if (hasProvinces)
        {
            return BadRequest("Cannot delete country that has provinces.");
        }

        _context.Countries.Remove(country);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa quốc gia");
        }

        return NoContent();
    }

    private bool CountryExists(Guid key)
    {
        return _context.Countries.Any(e => e.CountryId == key);
    }
}

