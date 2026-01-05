using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Controllers;

[ApiController]
[Route("odata")]
public class EquipmentTypesController : ODataController
{
    private readonly ApplicationDbContext _context;

    public EquipmentTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("EquipmentTypes")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.EquipmentTypes);
    }

    [HttpGet("EquipmentTypes({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var equipmentType = _context.EquipmentTypes.FirstOrDefault(e => e.EquipmentTypeId == key);
        if (equipmentType == null)
        {
            return NotFound();
        }
        return Ok(equipmentType);
    }

    [HttpPost("EquipmentTypes")]
    public async Task<IActionResult> Post([FromBody] EquipmentType equipmentType)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        equipmentType.EquipmentTypeId = equipmentType.EquipmentTypeId == Guid.Empty ? Guid.NewGuid() : equipmentType.EquipmentTypeId;
        _context.EquipmentTypes.Add(equipmentType);
        await _context.SaveChangesAsync();

        return Created($"odata/EquipmentTypes({equipmentType.EquipmentTypeId})", equipmentType);
    }

    [HttpPut("EquipmentTypes({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] EquipmentType equipmentType)
    {
        if (key != equipmentType.EquipmentTypeId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(equipmentType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EquipmentTypeExists(key))
            {
                return NotFound();
            }
            throw;
        }

        return Updated(equipmentType);
    }

    [HttpDelete("EquipmentTypes({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var equipmentType = await _context.EquipmentTypes.FindAsync(key);
        if (equipmentType == null)
        {
            return NotFound();
        }

        _context.EquipmentTypes.Remove(equipmentType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EquipmentTypeExists(Guid key)
    {
        return _context.EquipmentTypes.Any(e => e.EquipmentTypeId == key);
    }
}

