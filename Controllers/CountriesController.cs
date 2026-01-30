using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
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

        country.CountryId = country.CountryId == Guid.Empty ? Guid.NewGuid() : country.CountryId;
        if (string.IsNullOrEmpty(country.Status))
        {
            country.Status = "Active";
        }

        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

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
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CountryExists(Guid key)
    {
        return _context.Countries.Any(e => e.CountryId == key);
    }
}

