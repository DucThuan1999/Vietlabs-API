using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VietLab.Data;
using VietLab.Models;
using VietLab.Services;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata")]
public class ClientHistoriesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly IClientHistoryService _clientHistoryService;
    private readonly ILogger<ClientHistoriesController> _logger;

    public ClientHistoriesController(
        ApplicationDbContext context,
        IClientHistoryService clientHistoryService,
        ILogger<ClientHistoriesController> logger)
    {
        _context = context;
        _clientHistoryService = clientHistoryService;
        _logger = logger;
    }

    private Guid? GetCurrentAccountId()
    {
        var accountIdClaim = User.FindFirst("AccountId")?.Value;
        if (Guid.TryParse(accountIdClaim, out var accountId))
        {
            return accountId;
        }
        return null;
    }

    [HttpGet("ClientHistories")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.ClientHistories
            .Include(ch => ch.Client)
            .Include(ch => ch.ChangedByAccount)
                .ThenInclude(a => a!.Employee));
    }

    [HttpGet("ClientHistories({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var history = _context.ClientHistories
            .Include(ch => ch.Client)
            .Include(ch => ch.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefault(ch => ch.ClientHistoryId == key);

        if (history == null)
        {
            return NotFound();
        }

        return Ok(history);
    }

    [HttpPost("ClientHistories")]
    public async Task<IActionResult> Post([FromBody] ClientHistory clientHistory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate ClientId exists
        var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == clientHistory.ClientId);
        if (!clientExists)
        {
            return BadRequest($"Client with ID {clientHistory.ClientId} does not exist.");
        }

        // Validate ChangedByAccountId exists
        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == clientHistory.ChangedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {clientHistory.ChangedByAccountId} does not exist.");
        }

        // Set default values
        clientHistory.ClientHistoryId = clientHistory.ClientHistoryId == Guid.Empty ? Guid.NewGuid() : clientHistory.ClientHistoryId;
        if (clientHistory.ChangedDate == default)
        {
            clientHistory.ChangedDate = DateTime.UtcNow;
        }
        if (string.IsNullOrEmpty(clientHistory.ChangeType))
        {
            clientHistory.ChangeType = "Manual";
        }

        _context.ClientHistories.Add(clientHistory);
        await _context.SaveChangesAsync();

        // Reload với đầy đủ navigation properties
        var createdHistory = await _context.ClientHistories
            .Include(ch => ch.Client)
            .Include(ch => ch.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(ch => ch.ClientHistoryId == clientHistory.ClientHistoryId);

        return Created($"odata/ClientHistories({clientHistory.ClientHistoryId})", createdHistory);
    }

    [HttpPut("ClientHistories({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] ClientHistory clientHistory)
    {
        if (key != clientHistory.ClientHistoryId)
        {
            return BadRequest("Key mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate ClientId exists
        var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == clientHistory.ClientId);
        if (!clientExists)
        {
            return BadRequest($"Client with ID {clientHistory.ClientId} does not exist.");
        }

        // Validate ChangedByAccountId exists
        var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == clientHistory.ChangedByAccountId);
        if (!accountExists)
        {
            return BadRequest($"Account with ID {clientHistory.ChangedByAccountId} does not exist.");
        }

        var existingHistory = await _context.ClientHistories.FindAsync(key);
        if (existingHistory == null)
        {
            return NotFound();
        }

        _context.Entry(existingHistory).State = EntityState.Detached;
        _context.Entry(clientHistory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClientHistoryExists(key))
            {
                return NotFound();
            }
            throw;
        }

        // Reload với đầy đủ navigation properties
        var updatedHistory = await _context.ClientHistories
            .Include(ch => ch.Client)
            .Include(ch => ch.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .FirstOrDefaultAsync(ch => ch.ClientHistoryId == key);

        return Updated(updatedHistory);
    }

    [HttpDelete("ClientHistories({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var clientHistory = await _context.ClientHistories.FindAsync(key);
        if (clientHistory == null)
        {
            return NotFound();
        }

        _context.ClientHistories.Remove(clientHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClientHistoryExists(Guid key)
    {
        return _context.ClientHistories.Any(e => e.ClientHistoryId == key);
    }
}

