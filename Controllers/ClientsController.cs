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

[ApiController]
[Route("odata")]
public class ClientsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly IClientHistoryService _clientHistoryService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        ApplicationDbContext context,
        IClientHistoryService clientHistoryService,
        ILogger<ClientsController> logger)
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
    [Authorize]
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

        // Log client creation
        var accountId = GetCurrentAccountId();
        if (accountId.HasValue)
        {
            await _clientHistoryService.LogClientChangeAsync(
                client.ClientId,
                $"Tạo mới khách hàng: {client.CompanyName}",
                accountId.Value,
                "Created");
        }

        return Created($"odata/Clients({client.ClientId})", client);
    }

    [HttpPut("Clients({key})")]
    [Authorize]
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

        // Get original client to compare changes
        var originalClient = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientId == key);

        if (originalClient == null)
        {
            return NotFound();
        }

        // Build change description
        var changes = new List<string>();
        if (originalClient.CompanyName != client.CompanyName)
            changes.Add($"Tên công ty: '{originalClient.CompanyName}' → '{client.CompanyName}'");
        if (originalClient.Status != client.Status)
            changes.Add($"Trạng thái: '{originalClient.Status}' → '{client.Status}'");
        if (originalClient.DiscountRate != client.DiscountRate)
            changes.Add($"Mức chiết khấu: {originalClient.DiscountRate}% → {client.DiscountRate}%");
        if (originalClient.IsBlacklisted != client.IsBlacklisted)
            changes.Add($"Blacklist: {(originalClient.IsBlacklisted ? "Có" : "Không")} → {(client.IsBlacklisted ? "Có" : "Không")}");

        var changeDescription = changes.Any()
            ? $"Cập nhật thông tin khách hàng: {string.Join("; ", changes)}"
            : "Cập nhật thông tin khách hàng";

        _context.Entry(client).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();

            // Log client update
            var accountId = GetCurrentAccountId();
            if (accountId.HasValue)
            {
                await _clientHistoryService.LogClientChangeAsync(
                    client.ClientId,
                    changeDescription,
                    accountId.Value,
                    "Updated");
            }
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
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var client = await _context.Clients.FindAsync(key);
        if (client == null)
        {
            return NotFound();
        }

        var companyName = client.CompanyName;
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        // Log client deletion
        var accountId = GetCurrentAccountId();
        if (accountId.HasValue)
        {
            await _clientHistoryService.LogClientChangeAsync(
                key,
                $"Xóa khách hàng: {companyName}",
                accountId.Value,
                "Deleted");
        }

        return NoContent();
    }

    private bool ClientExists(Guid key)
    {
        return _context.Clients.Any(e => e.ClientId == key);
    }
}

