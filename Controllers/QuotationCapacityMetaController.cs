using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models.DTOs;

namespace VietLab.Controllers;

/// <summary>
/// Năng lực/chỉ định cho bộ lọc báo giá — join + gom nhóm theo AnalysisItemId ngay ở backend.
/// Thay cho việc FE tải 4 bảng whole-table (DepartmentAnalysisCapabilities/Designations,
/// SubcontractorCapabilities/Designations) rồi tự join bằng JS (xem useQuotationData.js phía FE).
/// Dữ liệu chung cho mọi báo giá (không theo từng báo giá) nên FE có thể cache dài hạn.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotationCapacityMetaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuotationCapacityMetaController(ApplicationDbContext context)
    {
        _context = context;
    }

    private record CapabilityRow(Guid CapabilityId, Guid AnalysisItemId, bool Nd107);
    private record DesignationJunctionRow(Guid CapabilityId, string? Name, string? DesignationCode, string? SymbolCode);

    [HttpGet]
    public async Task<ActionResult<QuotationCapacityMetaResponse>> Get(CancellationToken ct)
    {
        var dacCapList = await _context.DepartmentAnalysisCapabilities
            .AsNoTracking()
            .Where(c => c.Status == "Active")
            .Select(c => new CapabilityRow(c.DepartmentAnalysisCapabilityId, c.AnalysisItemId, c.Nd107))
            .ToListAsync(ct);

        var scCapList = await _context.SubcontractorCapabilities
            .AsNoTracking()
            .Where(c => c.Status == "Active")
            .Select(c => new CapabilityRow(c.SubcontractorCapabilityId, c.AnalysisItemId, c.Nd107))
            .ToListAsync(ct);

        var dacDList = await _context.DepartmentAnalysisCapabilityDesignations
            .AsNoTracking()
            .Select(d => new DesignationJunctionRow(
                d.DepartmentAnalysisCapabilityId,
                d.Designation!.Name,
                d.Designation.DesignationCode,
                d.Designation.SymbolCode))
            .ToListAsync(ct);

        var scdList = await _context.SubcontractorCapabilityDesignations
            .AsNoTracking()
            .Select(d => new DesignationJunctionRow(
                d.SubcontractorCapabilityId,
                d.Designation!.Name,
                d.Designation.DesignationCode,
                d.Designation.SymbolCode))
            .ToListAsync(ct);

        var capIdToAnalysisItemId = dacCapList.ToDictionary(c => c.CapabilityId, c => c.AnalysisItemId);
        var scIdToAnalysisItemId = scCapList.ToDictionary(c => c.CapabilityId, c => c.AnalysisItemId);

        var vietlabsDesignationsByItem = new Dictionary<Guid, HashSet<string>>();
        foreach (var row in dacDList)
        {
            var label = row.Name ?? row.DesignationCode;
            if (string.IsNullOrWhiteSpace(label)) continue;
            if (!capIdToAnalysisItemId.TryGetValue(row.CapabilityId, out var aid)) continue;
            AddToSet(vietlabsDesignationsByItem, aid, label);
        }

        var subcontractorDesignationsByItem = new Dictionary<Guid, HashSet<string>>();
        foreach (var row in scdList)
        {
            var label = row.Name ?? row.DesignationCode;
            if (string.IsNullOrWhiteSpace(label)) continue;
            if (!scIdToAnalysisItemId.TryGetValue(row.CapabilityId, out var aid)) continue;
            AddToSet(subcontractorDesignationsByItem, aid, label);
        }

        var designationSymbolsByDepartmentCapabilityId = new Dictionary<Guid, HashSet<string>>();
        var designationSymbolsByAnalysisItemIdVietlabs = new Dictionary<Guid, HashSet<string>>();
        foreach (var row in dacDList)
        {
            var symbol = row.SymbolCode?.Trim();
            if (string.IsNullOrEmpty(symbol)) continue;
            AddToSet(designationSymbolsByDepartmentCapabilityId, row.CapabilityId, symbol);
            if (capIdToAnalysisItemId.TryGetValue(row.CapabilityId, out var aid))
                AddToSet(designationSymbolsByAnalysisItemIdVietlabs, aid, symbol);
        }

        var designationSymbolsBySubcontractorCapabilityId = new Dictionary<Guid, HashSet<string>>();
        var designationSymbolsByAnalysisItemIdSubcontractor = new Dictionary<Guid, HashSet<string>>();
        foreach (var row in scdList)
        {
            var symbol = row.SymbolCode?.Trim();
            if (string.IsNullOrEmpty(symbol)) continue;
            AddToSet(designationSymbolsBySubcontractorCapabilityId, row.CapabilityId, symbol);
            if (scIdToAnalysisItemId.TryGetValue(row.CapabilityId, out var aid))
                AddToSet(designationSymbolsByAnalysisItemIdSubcontractor, aid, symbol);
        }

        var response = new QuotationCapacityMetaResponse
        {
            AnalysisItemIdsWithSubcontractorCapacity = scCapList.Select(c => c.AnalysisItemId).Distinct().ToList(),
            AnalysisItemIdsWithNd107Vietlabs = dacCapList.Where(c => c.Nd107).Select(c => c.AnalysisItemId).Distinct().ToList(),
            AnalysisItemIdsWithNd107Subcontractor = scCapList.Where(c => c.Nd107).Select(c => c.AnalysisItemId).Distinct().ToList(),
            DesignationsByAnalysisItemVietlabs = ToSortedListDict(vietlabsDesignationsByItem),
            DesignationsByAnalysisItemSubcontractor = ToSortedListDict(subcontractorDesignationsByItem),
            DesignationSymbolsByDepartmentCapabilityId = ToSortedListDict(designationSymbolsByDepartmentCapabilityId),
            DesignationSymbolsBySubcontractorCapabilityId = ToSortedListDict(designationSymbolsBySubcontractorCapabilityId),
            DesignationSymbolsByAnalysisItemIdVietlabs = ToSortedListDict(designationSymbolsByAnalysisItemIdVietlabs),
            DesignationSymbolsByAnalysisItemIdSubcontractor = ToSortedListDict(designationSymbolsByAnalysisItemIdSubcontractor),
        };

        return Ok(response);
    }

    private static void AddToSet(Dictionary<Guid, HashSet<string>> target, Guid key, string value)
    {
        if (!target.TryGetValue(key, out var set))
        {
            set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            target[key] = set;
        }
        set.Add(value);
    }

    private static Dictionary<string, List<string>> ToSortedListDict(Dictionary<Guid, HashSet<string>> source)
    {
        return source.ToDictionary(
            kv => kv.Key.ToString(),
            kv => kv.Value.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToList());
    }
}
