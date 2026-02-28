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
public class ContactsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(ApplicationDbContext context, ILogger<ContactsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("Contacts")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.Contacts);
    }

    [HttpGet("Contacts({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var contact = _context.Contacts.FirstOrDefault(c => c.ContactId == key);
        if (contact == null)
        {
            return NotFound();
        }
        return Ok(contact);
    }

    [HttpPost("Contacts")]
    public async Task<IActionResult> Post([FromBody] Contact contact)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        contact.ContactId = contact.ContactId == Guid.Empty ? Guid.NewGuid() : contact.ContactId;

        _context.Contacts.Add(contact);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu liên hệ");
        }

        return Created($"odata/Contacts({contact.ContactId})", contact);
    }

    [HttpPut("Contacts({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] Contact contact)
    {
        if (key != contact.ContactId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(contact).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContactExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật liên hệ");
        }

        return Updated(contact);
    }

    [HttpDelete("Contacts({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var contact = await _context.Contacts.FindAsync(key);
        if (contact == null)
        {
            return NotFound();
        }

        _context.Contacts.Remove(contact);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa liên hệ");
        }

        return NoContent();
    }

    private bool ContactExists(Guid key)
    {
        return _context.Contacts.Any(e => e.ContactId == key);
    }
}


