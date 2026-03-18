using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietLab.Models.DTOs;
using VietLab.Services;

namespace VietLab.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class PermissionMatrixController : ControllerBase
{
    private readonly ModulePermissionService _perm;

    public PermissionMatrixController(ModulePermissionService perm)
    {
        _perm = perm;
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog(CancellationToken ct)
    {
        var catalog = await _perm.GetCatalogAsync(ct);
        return Ok(new
        {
            modules = catalog.Modules,
            actions = catalog.Actions,
            availableCells = catalog.AvailableCells
        });
    }

    [HttpGet("accounts/{accountId:guid}/grants")]
    public async Task<IActionResult> GetGrants(Guid accountId, CancellationToken ct)
    {
        var grants = await _perm.GetGrantsForAccountAsync(accountId, ct);
        return Ok(new { grants });
    }

    /// <summary>Ma trận quyền của chính user đăng nhập — chỉ các ô đã được cấp.</summary>
    [HttpGet("me/granted-matrix")]
    public async Task<IActionResult> GetMyGrantedMatrix(CancellationToken ct)
    {
        var accountIdStr = User.FindFirst("AccountId")?.Value;
        if (string.IsNullOrEmpty(accountIdStr) || !Guid.TryParse(accountIdStr, out var accountId))
            return Unauthorized(new { message = "Không xác định được tài khoản." });
        var items = await _perm.GetGrantedMatrixItemsAsync(accountId, ct);
        return Ok(new { items });
    }

    [HttpPut("accounts/{accountId:guid}/grants")]
    public async Task<IActionResult> PutGrants(Guid accountId, [FromBody] ReplaceModuleGrantsRequest? body, CancellationToken ct)
    {
        if (body?.Grants == null)
            return BadRequest(new { message = "Thiếu danh sách grants." });
        var pairs = body.Grants
            .Where(g => !string.IsNullOrWhiteSpace(g.ModuleCode) && !string.IsNullOrWhiteSpace(g.ActionCode))
            .Select(g => new GrantPairDto(g.ModuleCode.Trim(), g.ActionCode.Trim()))
            .ToList();
        await _perm.ReplaceGrantsAsync(accountId, pairs, ct);
        var codes = await _perm.GetGrantedCodesAsync(accountId, ct);
        return Ok(new { grantedPermissionCodes = codes });
    }
}
