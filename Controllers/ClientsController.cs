using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using VietLab.Data;
using VietLab.Helpers;
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
        return Ok(_context.Clients
            .Include(c => c.Contacts)
            .Include(c => c.AgentClient)
            .Include(c => c.ClientIndustry));
    }

    /// <summary>
    /// Sinh mã khách hàng nội bộ kế tiếp dựa trên dữ liệu đầy đủ trên DB (không giới hạn 1000 bản ghi phía client).
    /// Nếu có <paramref name="agentClientId"/>: mã khách con = {Mã nội bộ đại lý}.{số thứ tự}, bỏ qua địa chỉ khách.
    /// </summary>
    [HttpGet("Clients/NextInternalCode")]
    [Authorize]
    public async Task<IActionResult> GetNextInternalCode(
        [FromQuery] string? customerType,
        [FromQuery] string? country,
        [FromQuery] string? province,
        [FromQuery] Guid? agentClientId)
    {
        if (string.IsNullOrWhiteSpace(customerType))
        {
            return BadRequest(new { message = "customerType là bắt buộc." });
        }

        if (agentClientId.HasValue && agentClientId.Value != Guid.Empty)
        {
            return await ComputeNextForAgentCustomerInternalCodeAsync(agentClientId.Value);
        }

        var areaCode = await ResolveAreaCodeAsync(province, country);
        if (string.IsNullOrEmpty(areaCode))
        {
            return BadRequest(new { message = "Không xác định được mã khu vực (tỉnh/quốc gia)." });
        }

        var ct = customerType.Trim();
        if (ct is "Cá nhân" or "Doanh nghiệp" or "Nhà nước")
        {
            return await ComputeNextStandardInternalCodeAsync(areaCode);
        }

        if (ct is "Đại lý" or "CTV")
        {
            return await ComputeNextAgentOrCtvInternalCodeAsync(areaCode);
        }

        return BadRequest(new { message = "Loại khách hàng không hợp lệ." });
    }

    [HttpGet("Clients({key})")]
    [EnableQuery]
    public IActionResult Get([FromRoute] Guid key)
    {
        var client = _context.Clients
            .Include(c => c.Contacts)
            .Include(c => c.AgentClient)
            .Include(c => c.ClientIndustry)
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

        if (!string.IsNullOrWhiteSpace(client.InternalCode) &&
            await InternalCodeInUseAsync(client.InternalCode.Trim(), excludeClientId: null))
        {
            return Conflict(new { message = "Mã khách hàng nội bộ đã tồn tại." });
        }

        client.ClientId = client.ClientId == Guid.Empty ? Guid.NewGuid() : client.ClientId;
        client.CreatedDate = DateTime.UtcNow;
        _context.Clients.Add(client);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "lưu khách hàng");
        }

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

        if (!string.IsNullOrWhiteSpace(client.InternalCode) &&
            await InternalCodeInUseAsync(client.InternalCode.Trim(), excludeClientId: key))
        {
            return Conflict(new { message = "Mã khách hàng nội bộ đã tồn tại." });
        }

        // Build change description
        var changes = new List<string>();
        if (originalClient.CompanyName != client.CompanyName)
            changes.Add($"Tên công ty: '{originalClient.CompanyName}' → '{client.CompanyName}'");
        if (originalClient.Status != client.Status)
            changes.Add($"Trạng thái: '{originalClient.Status}' → '{client.Status}'");
        if (originalClient.DiscountRate != client.DiscountRate)
            changes.Add($"Giảm giá: {originalClient.DiscountRate}% → {client.DiscountRate}%");
        if (originalClient.CommissionRate != client.CommissionRate)
            changes.Add($"Tỷ lệ hoa hồng: {originalClient.CommissionRate}% → {client.CommissionRate}%");
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
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "cập nhật khách hàng");
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
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return this.HandleDatabaseError(ex, _logger, "xóa khách hàng");
        }

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

    private async Task<bool> InternalCodeInUseAsync(string internalCode, Guid? excludeClientId)
    {
        if (string.IsNullOrWhiteSpace(internalCode))
        {
            return false;
        }

        var normalized = internalCode.Trim().ToLowerInvariant();
        return await _context.Clients.AsNoTracking().AnyAsync(c =>
            c.InternalCode != null &&
            (excludeClientId == null || c.ClientId != excludeClientId.Value) &&
            c.InternalCode.ToLower() == normalized);
    }

    private async Task<string?> ResolveAreaCodeAsync(string? province, string? country)
    {
        if (!string.IsNullOrWhiteSpace(province))
        {
            Province? provinceEntity;
            if (Guid.TryParse(province, out var provinceId))
            {
                provinceEntity = await _context.Provinces.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProvinceId == provinceId);
            }
            else
            {
                var pTrim = province.Trim();
                provinceEntity = await _context.Provinces.AsNoTracking()
                    .FirstOrDefaultAsync(p =>
                        p.Name == pTrim ||
                        (p.FullName != null && p.FullName == pTrim));
            }

            if (!string.IsNullOrWhiteSpace(provinceEntity?.ProvinceCode))
            {
                return provinceEntity!.ProvinceCode!.ToUpperInvariant();
            }
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            Country? countryEntity;
            if (Guid.TryParse(country, out var countryId))
            {
                countryEntity = await _context.Countries.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CountryId == countryId);
            }
            else
            {
                var cTrim = country.Trim();
                countryEntity = await _context.Countries.AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.FullNameVi == cTrim ||
                        c.NameEn == cTrim ||
                        c.FullNameEn == cTrim);
            }

            if (countryEntity != null)
            {
                if (!string.IsNullOrWhiteSpace(countryEntity.Alpha2))
                {
                    return countryEntity.Alpha2.ToUpperInvariant();
                }

                if (!string.IsNullOrWhiteSpace(countryEntity.Alpha3))
                {
                    return countryEntity.Alpha3.ToUpperInvariant();
                }
            }
        }

        return null;
    }

    private async Task<IActionResult> ComputeNextForAgentCustomerInternalCodeAsync(Guid agentClientId)
    {
        var agent = await _context.Clients.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientId == agentClientId);

        if (agent == null)
        {
            return NotFound(new { message = "Không tìm thấy đại lý/CTV." });
        }

        if (agent.CustomerType != "Đại lý" && agent.CustomerType != "CTV")
        {
            return BadRequest(new { message = "Khách được chọn không phải Đại lý/CTV." });
        }

        if (string.IsNullOrWhiteSpace(agent.InternalCode))
        {
            return BadRequest(new { message = "Đại lý/CTV chưa có mã khách hàng nội bộ." });
        }

        var prefix = agent.InternalCode.Trim();
        var prefixDot = prefix + ".";

        // Đếm mã đã có dạng {mã_agent}.{số} trên toàn DB — không lọc AgentClientId vì dữ liệu cũ có thể thiếu/sai FK.
        // Prefix kèm dấu chấm tránh nhầm N/CTH0001.* với N/CTH00010.*
        var codes = await _context.Clients.AsNoTracking()
            .Where(c => c.InternalCode != null && c.InternalCode.StartsWith(prefixDot))
            .Select(c => c.InternalCode!)
            .ToListAsync();

        var max = 0;
        foreach (var code in codes)
        {
            if (!code.StartsWith(prefixDot, StringComparison.Ordinal))
            {
                continue;
            }

            var suffix = code.Substring(prefixDot.Length);
            if (int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
            {
                max = Math.Max(max, n);
            }
        }

        var next = max + 1;
        return Ok(new { internalCode = $"{prefix}.{next}" });
    }

    private async Task<IActionResult> ComputeNextStandardInternalCodeAsync(string areaCode)
    {
        var totalLen = areaCode.Length + 5;
        var standardTypes = new[] { "Cá nhân", "Doanh nghiệp", "Nhà nước" };

        var candidates = await _context.Clients.AsNoTracking()
            .Where(c => c.InternalCode != null
                && c.InternalCode.Length == totalLen
                && c.InternalCode.StartsWith(areaCode)
                && c.CustomerType != null
                && standardTypes.Contains(c.CustomerType))
            .Select(c => c.InternalCode!)
            .ToListAsync();

        var max = 0;
        foreach (var code in candidates)
        {
            var suffix = code.Substring(areaCode.Length);
            if (suffix.Length == 5 && int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
            {
                max = Math.Max(max, n);
            }
        }

        var next = max + 1;
        if (next > 99999)
        {
            return BadRequest(new { message = "Đã hết dải mã (5 chữ số) cho khu vực này." });
        }

        return Ok(new { internalCode = $"{areaCode}{next.ToString("D5", CultureInfo.InvariantCulture)}" });
    }

    private async Task<IActionResult> ComputeNextAgentOrCtvInternalCodeAsync(string areaCode)
    {
        var prefix = "N/" + areaCode;
        var totalLen = prefix.Length + 4;

        var candidates = await _context.Clients.AsNoTracking()
            .Where(c => c.InternalCode != null
                && c.InternalCode.Length == totalLen
                && c.InternalCode.StartsWith(prefix)
                && !c.InternalCode.Contains('.')
                && (c.CustomerType == "Đại lý" || c.CustomerType == "CTV"))
            .Select(c => c.InternalCode!)
            .ToListAsync();

        var max = 0;
        foreach (var code in candidates)
        {
            var seqStr = code.Substring(prefix.Length);
            if (seqStr.Length == 4 && int.TryParse(seqStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
            {
                max = Math.Max(max, n);
            }
        }

        var next = max + 1;
        if (next > 9999)
        {
            return BadRequest(new { message = "Đã hết dải mã (4 chữ số) cho khu vực này." });
        }

        return Ok(new { internalCode = $"{prefix}{next.ToString("D4", CultureInfo.InvariantCulture)}" });
    }
}

