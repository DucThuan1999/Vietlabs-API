using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class AccountsController : ODataController
{
    private readonly ApplicationDbContext _context;

    public AccountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Accounts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Accounts);
    }

    [HttpGet("Accounts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var account = _context.Accounts.FirstOrDefault(a => a.AccountId == key);
        if (account == null)
        {
            return NotFound();
        }
        return Ok(account);
    }

    [HttpPost("Accounts")]
    public async Task<IActionResult> Post([FromBody] Account account)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        account.AccountId = account.AccountId == Guid.Empty ? Guid.NewGuid() : account.AccountId;
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return Created($"odata/Accounts({account.AccountId})", account);
    }

    [HttpPut("Accounts({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Account account)
    {
        if (key != account.AccountId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(account).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AccountExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(account);
    }

    [HttpDelete("Accounts({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var account = await _context.Accounts.FindAsync(key);
        if (account == null)
        {
            return NotFound();
        }

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AccountExists(Guid key)
    {
        return _context.Accounts.Any(a => a.AccountId == key);
    }
}


