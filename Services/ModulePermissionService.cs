using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Services;

public class ModulePermissionService
{
    private readonly ApplicationDbContext _db;

    public ModulePermissionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<string>> GetGrantedCodesAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _db.AccountModuleGrants
            .AsNoTracking()
            .Where(g => g.AccountId == accountId)
            .Join(_db.SecurityModules, g => g.SecurityModuleId, m => m.SecurityModuleId, (g, m) => new { g.MatrixActionId, ModuleCode = m.Code })
            .Join(_db.MatrixActions, x => x.MatrixActionId, a => a.MatrixActionId, (x, a) => x.ModuleCode + "." + a.Code)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<PermissionMatrixCatalogDto> GetCatalogAsync(CancellationToken ct = default)
    {
        var modules = await _db.SecurityModules
            .AsNoTracking()
            .Where(m => m.Status == "Active")
            .OrderBy(m => m.SortOrder)
            .Select(m => new ModuleRowDto(m.Code, m.NameVi, m.SortOrder, m.SecurityModuleId))
            .ToListAsync(ct);

        var actions = await _db.MatrixActions
            .AsNoTracking()
            .OrderBy(a => a.SortOrder)
            .Select(a => new ActionColDto(a.Code, a.NameVi, a.SortOrder, a.MatrixActionId))
            .ToListAsync(ct);

        var cells = await _db.SecurityModuleActions
            .AsNoTracking()
            .Join(_db.SecurityModules, sma => sma.SecurityModuleId, m => m.SecurityModuleId, (sma, m) => new { sma.MatrixActionId, ModuleCode = m.Code })
            .Join(_db.MatrixActions, x => x.MatrixActionId, a => a.MatrixActionId, (x, a) => new CellDto(x.ModuleCode, a.Code))
            .ToListAsync(ct);

        return new PermissionMatrixCatalogDto(modules, actions, cells);
    }

    public async Task<List<GrantPairDto>> GetGrantsForAccountAsync(Guid accountId, CancellationToken ct = default)
    {
        return await _db.AccountModuleGrants
            .AsNoTracking()
            .Where(g => g.AccountId == accountId)
            .Join(_db.SecurityModules, g => g.SecurityModuleId, m => m.SecurityModuleId, (g, m) => new { g.MatrixActionId, ModuleCode = m.Code })
            .Join(_db.MatrixActions, x => x.MatrixActionId, a => a.MatrixActionId, (x, a) => new GrantPairDto(x.ModuleCode, a.Code))
            .ToListAsync(ct);
    }

    /// <summary>Chỉ các ô ma trận đã cấp cho account (không trả ô trống).</summary>
    public async Task<List<GrantedMatrixItemDto>> GetGrantedMatrixItemsAsync(Guid accountId, CancellationToken ct = default)
    {
        var rows = await (
            from g in _db.AccountModuleGrants.AsNoTracking()
            where g.AccountId == accountId
            join m in _db.SecurityModules.AsNoTracking() on g.SecurityModuleId equals m.SecurityModuleId
            join a in _db.MatrixActions.AsNoTracking() on g.MatrixActionId equals a.MatrixActionId
            orderby m.SortOrder, a.SortOrder
            select new { m.Code, ModuleNameVi = m.NameVi, Ac = a.Code, ActionNameVi = a.NameVi }
        ).ToListAsync(ct);
        return rows
            .Select(x => new GrantedMatrixItemDto(
                x.Code + "." + x.Ac,
                x.Code,
                x.ModuleNameVi,
                x.Ac,
                x.ActionNameVi))
            .ToList();
    }

    /// <summary>Thay thế toàn bộ quyền ma trận của account (chỉ các ô hợp lệ trong catalog).</summary>
    public async Task ReplaceGrantsAsync(Guid accountId, IReadOnlyList<GrantPairDto> grants, CancellationToken ct = default)
    {
        var moduleByCode = await _db.SecurityModules
            .AsNoTracking()
            .Where(m => m.Status == "Active")
            .ToDictionaryAsync(m => m.Code, m => m.SecurityModuleId, ct);
        var actionByCode = await _db.MatrixActions.AsNoTracking().ToDictionaryAsync(a => a.Code, a => a.MatrixActionId, ct);
        var validKeys = (await _db.SecurityModuleActions
            .AsNoTracking()
            .Select(sma => new { sma.SecurityModuleId, sma.MatrixActionId })
            .ToListAsync(ct))
            .Select(v => (v.SecurityModuleId, v.MatrixActionId))
            .ToHashSet();

        var existing = await _db.AccountModuleGrants.Where(g => g.AccountId == accountId).ToListAsync(ct);
        _db.AccountModuleGrants.RemoveRange(existing);

        foreach (var g in grants)
        {
            if (!moduleByCode.TryGetValue(g.ModuleCode, out var mid) || !actionByCode.TryGetValue(g.ActionCode, out var aid))
                continue;
            if (!validKeys.Contains((mid, aid)))
                continue;
            _db.AccountModuleGrants.Add(new AccountModuleGrant
            {
                AccountModuleGrantId = Guid.NewGuid(),
                AccountId = accountId,
                SecurityModuleId = mid,
                MatrixActionId = aid
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}

public record ModuleRowDto(string Code, string NameVi, int SortOrder, Guid SecurityModuleId);
public record ActionColDto(string Code, string NameVi, int SortOrder, Guid MatrixActionId);
public record CellDto(string ModuleCode, string ActionCode);
public record GrantPairDto(string ModuleCode, string ActionCode);
public record GrantedMatrixItemDto(
    string PermissionCode,
    string ModuleCode,
    string ModuleNameVi,
    string ActionCode,
    string ActionNameVi);
public record PermissionMatrixCatalogDto(
    IReadOnlyList<ModuleRowDto> Modules,
    IReadOnlyList<ActionColDto> Actions,
    IReadOnlyList<CellDto> AvailableCells);
