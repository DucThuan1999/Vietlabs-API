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
public class ClientForecastsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClientForecastsController> _logger;

    public ClientForecastsController(ApplicationDbContext context, ILogger<ClientForecastsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("ClientForecasts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ClientForecasts
            .Include(cf => cf.Client)
            .Include(cf => cf.CreatedByAccount)
            .Include(cf => cf.UpdatedByAccount));
    }

    [HttpGet("ClientForecasts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var clientForecast = _context.ClientForecasts
            .Include(cf => cf.Client)
            .Include(cf => cf.CreatedByAccount)
            .Include(cf => cf.UpdatedByAccount)
            .FirstOrDefault(cf => cf.ClientForecastId == key);
        if (clientForecast == null)
        {
            return NotFound();
        }
        return Ok(clientForecast);
    }

    [HttpPost("ClientForecasts")]
    public async Task<IActionResult> Post([FromBody] ClientForecast clientForecast)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        clientForecast.ClientForecastId = clientForecast.ClientForecastId == Guid.Empty 
            ? Guid.NewGuid() 
            : clientForecast.ClientForecastId;
        clientForecast.CreatedAt = DateTime.UtcNow;
        _context.ClientForecasts.Add(clientForecast);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu dự báo khách hàng");
        }

        // Reload với đầy đủ navigation properties
        var createdForecast = await _context.ClientForecasts
            .Include(cf => cf.Client)
            .Include(cf => cf.CreatedByAccount)
            .Include(cf => cf.UpdatedByAccount)
            .FirstOrDefaultAsync(cf => cf.ClientForecastId == clientForecast.ClientForecastId);

        return Created($"odata/ClientForecasts({clientForecast.ClientForecastId})", createdForecast);
    }

    [HttpPut("ClientForecasts({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ClientForecast clientForecast)
    {
        if (key != clientForecast.ClientForecastId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        clientForecast.UpdatedAt = DateTime.UtcNow;
        _context.Entry(clientForecast).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientForecastExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật dự báo khách hàng");
        }

        // Reload với đầy đủ navigation properties
        var updatedForecast = await _context.ClientForecasts
            .Include(cf => cf.Client)
            .Include(cf => cf.CreatedByAccount)
            .Include(cf => cf.UpdatedByAccount)
            .FirstOrDefaultAsync(cf => cf.ClientForecastId == key);

        return Updated(updatedForecast);
    }

    [HttpDelete("ClientForecasts({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var clientForecast = await _context.ClientForecasts.FindAsync(key);
        if (clientForecast == null)
        {
            return NotFound();
        }

        _context.ClientForecasts.Remove(clientForecast);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa dự báo khách hàng");
        }

        return NoContent();
    }

    private bool ClientForecastExists(Guid key)
    {
        return _context.ClientForecasts.Any(e => e.ClientForecastId == key);
    }
}

