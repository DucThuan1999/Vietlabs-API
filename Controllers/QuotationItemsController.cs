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
public class QuotationItemsController : ODataController
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationItemsController> _logger;

    public QuotationItemsController(ApplicationDbContext context, ILogger<QuotationItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("QuotationItems")]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_context.QuotationItems
            .Include(qi => qi.Quotation)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package));
    }

    [HttpGet("QuotationItems({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var quotationItem = _context.QuotationItems
            .Include(qi => qi.Quotation)
            .Include(qi => qi.AnalysisItem)
            .Include(qi => qi.AnalysisGroup)
            .Include(qi => qi.Package)
            .FirstOrDefault(qi => qi.QuotationItemId == key);
        if (quotationItem == null)
        {
            return NotFound();
        }
        return Ok(quotationItem);
    }

    [HttpPost("QuotationItems")]
    public async Task<IActionResult> Post([FromBody] System.Text.Json.JsonElement body)
    {
        QuotationItem? quotationItem = null;
        
        try
        {
            // Nếu có field "quotationItem", lấy từ đó (OData format)
            if (body.TryGetProperty("quotationItem", out var quotationItemElement))
            {
                quotationItem = System.Text.Json.JsonSerializer.Deserialize<QuotationItem>(
                    quotationItemElement.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
            // Nếu không có wrapper, deserialize trực tiếp (frontend format)
            else
            {
                quotationItem = System.Text.Json.JsonSerializer.Deserialize<QuotationItem>(
                    body.GetRawText(),
                    new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
        }
        catch (System.Text.Json.JsonException ex)
        {
            return BadRequest(new { 
                error = "Invalid JSON format",
                message = $"Failed to parse request body: {ex.Message}"
            });
        }

        if (quotationItem == null)
        {
            return BadRequest(new { 
                error = "The quotationItem field is required",
                message = "Failed to deserialize QuotationItem from request body."
            });
        }

        // Validate và cleanup foreign keys theo constraint CK_quotation_items_single_reference
        // Chỉ một trong 3 foreign keys được phép có giá trị
        var itemType = quotationItem.ItemType?.Trim();
        
        if (string.IsNullOrEmpty(itemType))
        {
            return BadRequest(new { 
                error = "ItemType is required",
                message = "ItemType must be one of: AnalysisItem, AnalysisGroup, Package"
            });
        }

        // Clear các foreign keys không liên quan dựa trên ItemType
        switch (itemType.ToLower())
        {
            case "analysisitem":
                // Chỉ giữ AnalysisItemId, clear các FK khác
                if (quotationItem.AnalysisItemId == null || quotationItem.AnalysisItemId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "AnalysisItemId is required",
                        message = "AnalysisItemId is required when ItemType is 'AnalysisItem'"
                    });
                }
                // Nếu isStandalone = false, phải có analysisGroupId
                if (quotationItem.IsStandalone == false)
                {
                    if (quotationItem.AnalysisGroupId == null || quotationItem.AnalysisGroupId == Guid.Empty)
                    {
                        return BadRequest(new { 
                            error = "AnalysisGroupId is required",
                            message = "AnalysisGroupId is required when ItemType is 'AnalysisItem' and isStandalone is false"
                        });
                    }
                    // Giữ AnalysisGroupId, chỉ clear PackageId
                }
                else
                {
                    // Nếu isStandalone = true hoặc null, clear AnalysisGroupId
                    quotationItem.AnalysisGroupId = null;
                }
                quotationItem.PackageId = null;
                break;
                
            case "analysisgroup":
                // Chỉ giữ AnalysisGroupId, clear các FK khác
                if (quotationItem.AnalysisGroupId == null || quotationItem.AnalysisGroupId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "AnalysisGroupId is required",
                        message = "AnalysisGroupId is required when ItemType is 'AnalysisGroup'"
                    });
                }
                quotationItem.AnalysisItemId = null;
                quotationItem.PackageId = null;
                // IsStandalone không áp dụng cho AnalysisGroup
                quotationItem.IsStandalone = null;
                break;
                
            case "package":
                // Chỉ giữ PackageId, clear các FK khác
                if (quotationItem.PackageId == null || quotationItem.PackageId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "PackageId is required",
                        message = "PackageId is required when ItemType is 'Package'"
                    });
                }
                quotationItem.AnalysisItemId = null;
                quotationItem.AnalysisGroupId = null;
                // IsStandalone không áp dụng cho Package
                quotationItem.IsStandalone = null;
                break;
                
            default:
                return BadRequest(new { 
                    error = "Invalid ItemType",
                    message = $"ItemType must be one of: AnalysisItem, AnalysisGroup, Package. Received: {itemType}"
                });
        }

        // Validate model
        if (!TryValidateModel(quotationItem))
        {
            return BadRequest(ModelState);
        }

        quotationItem.QuotationItemId = quotationItem.QuotationItemId == Guid.Empty ? Guid.NewGuid() : quotationItem.QuotationItemId;
        quotationItem.CreatedAt = DateTime.UtcNow;
        _context.QuotationItems.Add(quotationItem);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu mục báo giá");
        }

        return Created($"odata/QuotationItems({quotationItem.QuotationItemId})", quotationItem);
    }

    [HttpPut("QuotationItems({key})")]
    public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] QuotationItem quotationItem)
    {
        if (key != quotationItem.QuotationItemId)
        {
            return BadRequest();
        }

        // Validate và cleanup foreign keys theo constraint CK_quotation_items_single_reference
        var itemType = quotationItem.ItemType?.Trim();
        
        if (string.IsNullOrEmpty(itemType))
        {
            return BadRequest(new { 
                error = "ItemType is required",
                message = "ItemType must be one of: AnalysisItem, AnalysisGroup, Package"
            });
        }

        // Clear các foreign keys không liên quan dựa trên ItemType
        switch (itemType.ToLower())
        {
            case "analysisitem":
                if (quotationItem.AnalysisItemId == null || quotationItem.AnalysisItemId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "AnalysisItemId is required",
                        message = "AnalysisItemId is required when ItemType is 'AnalysisItem'"
                    });
                }
                // Nếu isStandalone = false, phải có analysisGroupId
                if (quotationItem.IsStandalone == false)
                {
                    if (quotationItem.AnalysisGroupId == null || quotationItem.AnalysisGroupId == Guid.Empty)
                    {
                        return BadRequest(new { 
                            error = "AnalysisGroupId is required",
                            message = "AnalysisGroupId is required when ItemType is 'AnalysisItem' and isStandalone is false"
                        });
                    }
                    // Giữ AnalysisGroupId, chỉ clear PackageId
                }
                else
                {
                    // Nếu isStandalone = true hoặc null, clear AnalysisGroupId
                    quotationItem.AnalysisGroupId = null;
                }
                quotationItem.PackageId = null;
                break;
                
            case "analysisgroup":
                if (quotationItem.AnalysisGroupId == null || quotationItem.AnalysisGroupId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "AnalysisGroupId is required",
                        message = "AnalysisGroupId is required when ItemType is 'AnalysisGroup'"
                    });
                }
                quotationItem.AnalysisItemId = null;
                quotationItem.PackageId = null;
                quotationItem.IsStandalone = null;
                break;
                
            case "package":
                if (quotationItem.PackageId == null || quotationItem.PackageId == Guid.Empty)
                {
                    return BadRequest(new { 
                        error = "PackageId is required",
                        message = "PackageId is required when ItemType is 'Package'"
                    });
                }
                quotationItem.AnalysisItemId = null;
                quotationItem.AnalysisGroupId = null;
                quotationItem.IsStandalone = null;
                break;
                
            default:
                return BadRequest(new { 
                    error = "Invalid ItemType",
                    message = $"ItemType must be one of: AnalysisItem, AnalysisGroup, Package. Received: {itemType}"
                });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        quotationItem.UpdatedAt = DateTime.UtcNow;
        _context.Entry(quotationItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuotationItemExists(key))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật mục báo giá");
        }

        return Updated(quotationItem);
    }

    [HttpDelete("QuotationItems({key})")]
    public async Task<IActionResult> Delete([FromRoute] Guid key)
    {
        var quotationItem = await _context.QuotationItems.FindAsync(key);
        if (quotationItem == null)
        {
            return NotFound();
        }

        _context.QuotationItems.Remove(quotationItem);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa mục báo giá");
        }

        return NoContent();
    }

    private bool QuotationItemExists(Guid key)
    {
        return _context.QuotationItems.Any(e => e.QuotationItemId == key);
    }
}

