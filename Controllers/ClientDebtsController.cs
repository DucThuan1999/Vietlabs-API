using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class ClientDebtsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public ClientDebtsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("ClientDebts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ClientDebts
            .Include(cd => cd.Client));
    }

    [HttpGet("ClientDebts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var clientDebt = _context.ClientDebts
            .Include(cd => cd.Client)
            .FirstOrDefault(cd => cd.ClientDebtId == key);
        if (clientDebt == null)
        {
            return NotFound();
        }
        return Ok(clientDebt);
    }

    [HttpPost("ClientDebts")]
    public async Task<IActionResult> Post([FromBody] ClientDebt clientDebt)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        clientDebt.ClientDebtId = clientDebt.ClientDebtId == Guid.Empty ? Guid.NewGuid() : clientDebt.ClientDebtId;
        clientDebt.CreatedAt = DateTime.UtcNow;
        _context.ClientDebts.Add(clientDebt);
        await _context.SaveChangesAsync();

        return Created($"odata/ClientDebts({clientDebt.ClientDebtId})", clientDebt);
    }

    [HttpPut("ClientDebts({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ClientDebt clientDebt)
    {
        if (key != clientDebt.ClientDebtId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        clientDebt.UpdatedAt = DateTime.UtcNow;
        _context.Entry(clientDebt).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientDebtExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(clientDebt);
    }

    [HttpDelete("ClientDebts({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var clientDebt = await _context.ClientDebts.FindAsync(key);
        if (clientDebt == null)
        {
            return NotFound();
        }

        _context.ClientDebts.Remove(clientDebt);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientDebtExists(Guid key)
    {
        return _context.ClientDebts.Any(e => e.ClientDebtId == key);
    }
}

