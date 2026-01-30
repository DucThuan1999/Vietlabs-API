using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class ClientForecastsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public ClientForecastsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("ClientForecasts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ClientForecasts
            .Include(cf => cf.Client));
    }

    [HttpGet("ClientForecasts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var clientForecast = _context.ClientForecasts
            .Include(cf => cf.Client)
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
        await _context.SaveChangesAsync();

        return Created($"odata/ClientForecasts({clientForecast.ClientForecastId})", clientForecast);
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

        return Updated(clientForecast);
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
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientForecastExists(Guid key)
    {
        return _context.ClientForecasts.Any(e => e.ClientForecastId == key);
    }
}

