using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietLab.Models.DTOs;
using VietLab.Services;

namespace VietLab.Controllers;

/// <summary>
/// Endpoint inbound: AMIS/MISA gọi callback về VietLab sau khi xử lý chứng từ/dữ liệu.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AmisCallbacksController : ControllerBase
{
    private readonly IAmisCallbackService _callbackService;
    private readonly ILogger<AmisCallbacksController> _logger;

    public AmisCallbacksController(
        IAmisCallbackService callbackService,
        ILogger<AmisCallbacksController> logger)
    {
        _callbackService = callbackService;
        _logger = logger;
    }

    /// <summary>
    /// Callback từ AMIS ACT Open — validate chữ ký HMAC-SHA256(data, AppId), lưu log, trả response MISA.
    /// </summary>
    [HttpPost("call_back_data")]
    public async Task<AmisCallbackDataOutput> CallBackData(
        [FromBody] AmisCallbackDataInput param,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _callbackService.HandleCallbackAsync(param, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMIS CallBackData endpoint failed");
            return new AmisCallbackDataOutput
            {
                Success = false,
                ErrorCode = "Exception",
                ErrorMessage = ex.Message,
            };
        }
    }
}
