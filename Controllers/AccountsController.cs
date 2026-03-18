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
public class AccountsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(ApplicationDbContext context, ILogger<AccountsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static IQueryable<Account> AccountsWithEmployeeGraph(IQueryable<Account> query) =>
        query
            .Include(a => a.Employee)!.ThenInclude(e => e!.Department)
            .Include(a => a.Employee)!.ThenInclude(e => e!.EmployeeTitle);

    [HttpGet("Accounts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(AccountsWithEmployeeGraph(_context.Accounts));
    }

    [HttpGet("Accounts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var account = AccountsWithEmployeeGraph(_context.Accounts)
            .FirstOrDefault(a => a.AccountId == key);
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu tài khoản");
        }

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

        // Detach bất kỳ Account nào cùng key đang bị track (vd. body bị attach bởi pipeline) để tránh trùng
        foreach (var entry in _context.ChangeTracker.Entries<Account>().ToList())
        {
            if (entry.Entity.AccountId == key)
            {
                entry.State = EntityState.Detached;
            }
        }

        var existing = await _context.Accounts.FindAsync(key);
        if (existing == null)
        {
            return NotFound();
        }

        // Chỉ cập nhật scalar/FK từ body, không dùng navigation từ body
        existing.EmployeeId = account.EmployeeId;
        existing.UserName = account.UserName;
        // Body PATCH kiểu (form nhân viên) thường không gửi PasswordHash → null xóa mật khẩu.
        // Chỉ đổi hash khi client gửi giá trị thật (đổi mật khẩu).
        if (!string.IsNullOrEmpty(account.PasswordHash))
        {
            existing.PasswordHash = account.PasswordHash;
        }
        existing.Status = account.Status;

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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật tài khoản");
        }

        return Updated(existing);
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa tài khoản");
        }

        return NoContent();
    }

    private bool AccountExists(Guid key)
    {
        return _context.Accounts.Any(a => a.AccountId == key);
    }
}


