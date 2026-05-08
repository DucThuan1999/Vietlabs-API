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
public class OrderSamplesController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderSamplesController> _logger;

    public OrderSamplesController(ApplicationDbContext context, ILogger<OrderSamplesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("OrderSamples")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.OrderSamples
            .Include(os => os.Order)
            .Include(os => os.SampleMatrix));
    }

    [HttpGet("OrderSamples({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var orderSample = _context.OrderSamples
            .Include(os => os.Order)
            .Include(os => os.SampleMatrix)
            .FirstOrDefault(os => os.OrderSampleId == key);

        if (orderSample == null)
        {
            return NotFound();
        }

        return Ok(orderSample);
    }

    [HttpPost("OrderSamples")]
    public async Task<IActionResult> Post([FromBody] OrderSample orderSample)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        orderSample.OrderSampleId = orderSample.OrderSampleId == Guid.Empty ? Guid.NewGuid() : orderSample.OrderSampleId;
        _context.OrderSamples.Add(orderSample);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu thông tin mẫu");
        }

        var created = await _context.OrderSamples
            .Include(os => os.Order)
            .Include(os => os.SampleMatrix)
            .FirstOrDefaultAsync(os => os.OrderSampleId == orderSample.OrderSampleId);

        return Created($"odata/OrderSamples({orderSample.OrderSampleId})", created);
    }

    [HttpPut("OrderSamples({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderSample orderSample)
    {
        if (key != orderSample.OrderSampleId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(orderSample).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderSampleExists(key))
            {
                return NotFound();
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật thông tin mẫu");
        }

        var updated = await _context.OrderSamples
            .Include(os => os.Order)
            .Include(os => os.SampleMatrix)
            .FirstOrDefaultAsync(os => os.OrderSampleId == key);

        return Updated(updated);
    }

    [HttpDelete("OrderSamples({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var orderSample = await _context.OrderSamples.FindAsync(key);
        if (orderSample == null)
        {
            return NotFound();
        }

        _context.OrderSamples.Remove(orderSample);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa thông tin mẫu");
        }

        return NoContent();
    }

    private bool OrderSampleExists(Guid key)
    {
        return _context.OrderSamples.Any(e => e.OrderSampleId == key);
    }
}

