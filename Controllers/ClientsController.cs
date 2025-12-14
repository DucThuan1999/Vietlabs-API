using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class ClientsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Clients")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Clients.Include(c => c.Contacts));
    }

    [HttpGet("Clients({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var client = _context.Clients
            .Include(c => c.Contacts)
            .FirstOrDefault(c => c.ClientId == key);
        if (client == null)
        {
            return NotFound();
        }
        return Ok(client);
    }

    [HttpPost("Clients")]
    public async Task<IActionResult> Post([FromBody] Client client)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        client.ClientId = client.ClientId == Guid.Empty ? Guid.NewGuid() : client.ClientId;
        client.CreatedDate = DateTime.UtcNow;
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return Created($"odata/Clients({client.ClientId})", client);
    }

    [HttpPut("Clients({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Client client)
    {
        if (key != client.ClientId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(client).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(client);
    }

    [HttpDelete("Clients({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var client = await _context.Clients.FindAsync(key);
        if (client == null)
        {
            return NotFound();
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientExists(Guid key)
    {
        return _context.Clients.Any(e => e.ClientId == key);
    }
}

