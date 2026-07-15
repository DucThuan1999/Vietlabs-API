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
public class WardsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WardsController> _logger;

    public WardsController(
        ApplicationDbContext context,
        ILogger<WardsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Wards")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Wards
            .Include(w => w.Province)
            .Include(w => w.Country));
    }
    
    // Endpoint tiện ích: Lấy tất cả wards theo provinceId
    [HttpGet("Wards/ByProvince({provinceId})")]
    [EnableQuery]
    public IActionResult GetByProvince([FromRoute] Guid provinceId)
    {
        var wards = _context.Wards
            .Where(w => w.ProvinceId == provinceId)
            .AsNoTracking();
        
        return Ok(wards);
    }
    
    // Endpoint tiện ích: Lấy tất cả wards theo countryId
    [HttpGet("Wards/ByCountry({countryId})")]
    [EnableQuery]
    public IActionResult GetByCountry([FromRoute] Guid countryId)
    {
        var wards = _context.Wards
            .Where(w => w.CountryId == countryId)
            .AsNoTracking();
        
        return Ok(wards);
    }

    [HttpGet("Wards({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var ward = _context.Wards
            .Include(w => w.Province)
            .Include(w => w.Country)
            .FirstOrDefault(w => w.WardId == key);

        if (ward == null)
        {
            return NotFound();
        }

        return Ok(ward);
    }

    [HttpPost("Wards")]
    public async Task<IActionResult> Post([FromBody] Ward ward)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate ProvinceId exists
        var provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == ward.ProvinceId);
        if (!provinceExists)
        {
            return BadRequest($"Province with ID {ward.ProvinceId} does not exist.");
        }

        // Validate CountryId exists
        var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == ward.CountryId);
        if (!countryExists)
        {
            return BadRequest($"Country with ID {ward.CountryId} does not exist.");
        }

        // Validate Province belongs to Country
        var province = await _context.Provinces.FindAsync(ward.ProvinceId);
        if (province != null && province.CountryId != ward.CountryId)
        {
            return BadRequest("Province does not belong to the specified country.");
        }

        ward.WardId = ward.WardId == Guid.Empty ? Guid.NewGuid() : ward.WardId;
        if (string.IsNullOrEmpty(ward.Status))
        {
            ward.Status = "Active";
        }

        _context.Wards.Add(ward);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu phường/xã");
        }

        return Created($"odata/Wards({ward.WardId})", ward);
    }

    [HttpPut("Wards({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Ward ward)
    {
        if (key != ward.WardId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate ProvinceId exists
        var provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceId == ward.ProvinceId);
        if (!provinceExists)
        {
            return BadRequest($"Province with ID {ward.ProvinceId} does not exist.");
        }

        // Validate CountryId exists
        var countryExists = await _context.Countries.AnyAsync(c => c.CountryId == ward.CountryId);
        if (!countryExists)
        {
            return BadRequest($"Country with ID {ward.CountryId} does not exist.");
        }

        // Validate Province belongs to Country
        var province = await _context.Provinces.FindAsync(ward.ProvinceId);
        if (province != null && province.CountryId != ward.CountryId)
        {
            return BadRequest("Province does not belong to the specified country.");
        }

        _context.Entry(ward).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WardExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật phường/xã");
        }

        return Updated(ward);
    }

    [HttpDelete("Wards({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var ward = await _context.Wards.FindAsync(key);
        if (ward == null)
        {
            return NotFound();
        }

        _context.Wards.Remove(ward);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa phường/xã");
        }

        return NoContent();
    }

    private bool WardExists(Guid key)
    {
        return _context.Wards.Any(e => e.WardId == key);
    }
}

