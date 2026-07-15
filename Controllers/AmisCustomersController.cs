using Microsoft.AspNetCore.Mvc;
using VietLab.Models.DTOs;
using VietLab.Services;

namespace VietLab.Controllers;

/// <summary>
/// VietLab gọi AMIS/MISA: tạo khách hàng, lấy khách hàng, lấy công nợ.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AmisCustomersController : ControllerBase
{
    private readonly IAmisAccountingService _amis;
    private readonly ILogger<AmisCustomersController> _logger;

    public AmisCustomersController(IAmisAccountingService amis, ILogger<AmisCustomersController> logger)
    {
        _amis = amis;
        _logger = logger;
    }

    /// <summary>Tạo khách hàng (đối tượng) trên AMIS — save_dictionary, dictionary_type = 1.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateAmisCustomerRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _amis.CreateCustomerAsync(request, cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMIS CreateCustomer failed");
            return StatusCode(502, new { success = false, errorMessage = ex.Message });
        }
    }

    /// <summary>Lấy danh sách khách hàng từ AMIS — get_dictionary, data_type = 1.</summary>
    [HttpGet]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromQuery] string? lastSyncTime = null,
        [FromQuery] string? branchId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _amis.GetCustomersAsync(new AmisPagedQuery
            {
                Skip = skip,
                Take = take,
                LastSyncTime = lastSyncTime,
                BranchId = branchId,
            }, cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMIS GetCustomers failed");
            return StatusCode(502, new { success = false, errorMessage = ex.Message });
        }
    }

    /// <summary>Lấy công nợ đối tượng từ AMIS — get_list_acc_obj_debt (mặc định dataType=0 phải thu).</summary>
    [HttpGet("debts")]
    public async Task<IActionResult> GetDebts(
        [FromQuery] int dataType = AmisAccountingService.DebtReceivableDataType,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromQuery] string? lastSyncTime = null,
        [FromQuery] string? branchId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _amis.GetCustomerDebtsAsync(new AmisDebtQuery
            {
                DataType = dataType,
                Skip = skip,
                Take = take,
                LastSyncTime = lastSyncTime,
                BranchId = branchId,
            }, cancellationToken);
            return ToActionResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMIS GetDebts failed");
            return StatusCode(502, new { success = false, errorMessage = ex.Message });
        }
    }

    private IActionResult ToActionResult<T>(AmisOperationResult<T> result)
    {
        if (!result.Success)
        {
            return BadRequest(new
            {
                success = false,
                errorCode = result.ErrorCode,
                errorMessage = result.ErrorMessage,
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Data,
            customData = result.CustomData,
        });
    }
}
