using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietLab.Models;

namespace VietLab.Data;

/// <summary>
/// Đồng bộ danh mục layer 0 phục vụ import năng lực (Capability.xlsx) và chỉ tiêu (CSV):
/// chỉ định (ISO, Cục BVTV, …), nhóm chỉ tiêu (NCT-*), kỹ thuật lab, phòng ban theo nhóm kỹ thuật.
/// Chi nhánh không seed ở đây — giả định DB đã có đủ chi nhánh (HCM, Cần Thơ, Bạc Liêu, Cà Mau, …).
/// Idempotent: theo DesignationCode / DepartmentCode / TechniqueCode.
/// </summary>
public sealed class Layer0ReferenceDataSeeder
{
    private readonly ILogger<Layer0ReferenceDataSeeder> _logger;

    public Layer0ReferenceDataSeeder(ILogger<Layer0ReferenceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SyncAsync(ApplicationDbContext db, CancellationToken cancellationToken = default)
    {
        await NormalizeLegacyDesignationCodesAsync(db, cancellationToken);
        await SyncDesignationsAsync(db, cancellationToken);
        await SyncSampleMatrixGroupsFromCapabilityXlsxAsync(db, cancellationToken);
        await SyncSampleMatricesFromCapabilityXlsxAsync(db, cancellationToken);
        await SyncStandardsFromCapabilityXlsxAsync(db, cancellationToken);
        await SyncReferenceMethodsFromCapabilityXlsxAsync(db, cancellationToken);
        await SyncEquipmentTypesFromCapabilityXlsxAsync(db, cancellationToken);
        await SyncAnalysisGroupsFromCapabilityImportAsync(db, cancellationToken);
        await SyncLaboratoryTechniquesAsync(db, cancellationToken);
        await SyncLaboratoryDisciplineDepartmentsAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Layer0 reference data sync completed.");
    }

    private static Guid StableGuid(string key)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(key));
        return new Guid(bytes);
    }

    private static string NormalizeSampleMatrixGroupKey(string nameVi) =>
        string.Join(' ', nameVi.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToLowerInvariant();

    /// <summary>
    /// Nhóm nền mẫu từ Capability.xlsx (sheet Vietlabs + NTP). Idempotent: theo tên (không phân biệt hoa thường) hoặc id cố định.
    /// </summary>
    private async Task SyncSampleMatrixGroupsFromCapabilityXlsxAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;
        var seeds = CapabilityImportRules.SampleMatrixGroupsFromCapabilityXlsx;

        for (var i = 0; i < seeds.Count; i++)
        {
            var (nameVi, nameEn) = seeds[i];
            var code = $"NNM-{i + 1:D4}";
            var nameKey = NormalizeSampleMatrixGroupKey(nameVi);
            var preferredId = StableGuid("vietlab:layer0:smg:" + nameKey);

            var byName = await db.SampleMatrixGroups
                .FirstOrDefaultAsync(
                    s => s.NameVi != null && s.NameVi.Trim().ToLower() == nameVi.Trim().ToLower(),
                    cancellationToken);

            if (byName is not null)
            {
                byName.NameVi = nameVi;
                byName.NameEn = nameEn;
                if (string.IsNullOrWhiteSpace(byName.SampleMatrixGroupCode))
                    byName.SampleMatrixGroupCode = code;
                byName.Status = "Active";
                byName.Notes = null;
                byName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: sample matrix group by name {Name} — updated", nameVi);
                continue;
            }

            if (await db.SampleMatrixGroups.AnyAsync(s => s.SampleMatrixGroupId == preferredId, cancellationToken))
            {
                var row = await db.SampleMatrixGroups.FirstAsync(s => s.SampleMatrixGroupId == preferredId, cancellationToken);
                row.NameVi = nameVi;
                row.NameEn = nameEn;
                row.SampleMatrixGroupCode = code;
                row.Status = "Active";
                row.Notes = null;
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: sample matrix group {Code} — updated", code);
                continue;
            }

            db.SampleMatrixGroups.Add(new SampleMatrixGroup
            {
                SampleMatrixGroupId = preferredId,
                SampleMatrixGroupCode = code,
                NameVi = nameVi,
                NameEn = nameEn,
                Status = "Active",
                Notes = null,
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted sample matrix group {Code} — {Name}", code, nameVi);
        }
    }

    /// <summary>
    /// Nền mẫu từ Capability.xlsx: distinct (nhóm, nền mẫu). Idempotent theo (group_id + name_vi) hoặc id cố định.
    /// </summary>
    private async Task SyncSampleMatricesFromCapabilityXlsxAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;
        var rows = CapabilitySampleMatricesSeedData.Rows;

        for (var i = 0; i < rows.Length; i++)
        {
            var (groupVi, matrixVi, registered) = rows[i];
            var code = $"NM-{i + 1:D4}";

            var group = await db.SampleMatrixGroups.FirstOrDefaultAsync(
                g => g.NameVi != null && g.NameVi.Trim().ToLower() == groupVi.Trim().ToLower(),
                cancellationToken);
            if (group is null)
            {
                _logger.LogWarning("Layer0: sample matrix skipped — unknown group \"{Group}\" for matrix \"{Matrix}\"", groupVi, matrixVi);
                continue;
            }

            var groupId = group.SampleMatrixGroupId;
            var pairKey = NormalizeSampleMatrixGroupKey(groupVi) + "|" + NormalizeSampleMatrixGroupKey(matrixVi);
            var preferredId = StableGuid("vietlab:layer0:sm:" + pairKey);

            var byGroupAndName = await db.SampleMatrices.FirstOrDefaultAsync(
                s => s.SampleMatrixGroupId == groupId
                     && s.NameVi != null
                     && s.NameVi.Trim().ToLower() == matrixVi.Trim().ToLower(),
                cancellationToken);

            if (byGroupAndName is not null)
            {
                byGroupAndName.NameVi = matrixVi;
                byGroupAndName.RegisteredMatrix = registered;
                byGroupAndName.Notes = null;
                byGroupAndName.NameEn = null;
                byGroupAndName.SampleMatrixCode = code;
                byGroupAndName.Status = "Active";
                byGroupAndName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: sample matrix updated (by group+name) — {Matrix}", matrixVi);
                continue;
            }

            if (await db.SampleMatrices.AnyAsync(s => s.SampleMatrixId == preferredId, cancellationToken))
            {
                var row = await db.SampleMatrices.FirstAsync(s => s.SampleMatrixId == preferredId, cancellationToken);
                row.SampleMatrixGroupId = groupId;
                row.NameVi = matrixVi;
                row.RegisteredMatrix = registered;
                row.SampleMatrixCode = code;
                row.Status = "Active";
                row.Notes = null;
                row.NameEn = null;
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: sample matrix {Code} — updated", code);
                continue;
            }

            db.SampleMatrices.Add(new SampleMatrix
            {
                SampleMatrixId = preferredId,
                SampleMatrixGroupId = groupId,
                SampleMatrixCode = code,
                NameVi = matrixVi,
                NameEn = null,
                RegisteredMatrix = registered,
                Status = "Active",
                Notes = null,
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted sample matrix {Code} — {Name}", code, matrixVi);
        }
    }

    /// <summary>
    /// Quy chuẩn/tiêu chuẩn từ Capability.xlsx (distinct cột tương ứng). Idempotent theo tên hoặc id cố định. Mã TC-001… khớp form frontend.
    /// </summary>
    private async Task SyncStandardsFromCapabilityXlsxAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;
        var names = CapabilityStandardsSeedData.NameViList;

        for (var i = 0; i < names.Length; i++)
        {
            var nameVi = names[i];
            var code = $"TC-{i + 1:D3}";
            var nameKey = NormalizeSampleMatrixGroupKey(nameVi);
            var preferredId = StableGuid("vietlab:layer0:standard:" + nameKey);

            var byName = await db.Standards.FirstOrDefaultAsync(
                s => s.NameVi != null && s.NameVi.Trim().ToLower() == nameVi.Trim().ToLower(),
                cancellationToken);

            if (byName is not null)
            {
                byName.NameVi = nameVi;
                byName.StandardCode = code;
                byName.SequenceNumber = i + 1;
                byName.Status = "Active";
                byName.Notes = null;
                byName.NameEn = null;
                byName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: standard by name — updated {Name}", nameVi);
                continue;
            }

            if (await db.Standards.AnyAsync(s => s.StandardId == preferredId, cancellationToken))
            {
                var row = await db.Standards.FirstAsync(s => s.StandardId == preferredId, cancellationToken);
                row.NameVi = nameVi;
                row.StandardCode = code;
                row.SequenceNumber = i + 1;
                row.Status = "Active";
                row.Notes = null;
                row.NameEn = null;
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: standard {Code} — updated", code);
                continue;
            }

            db.Standards.Add(new Standard
            {
                StandardId = preferredId,
                StandardCode = code,
                SequenceNumber = i + 1,
                NameVi = nameVi,
                NameEn = null,
                Status = "Active",
                Notes = null,
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted standard {Code} — {Name}", code, nameVi);
        }
    }

    /// <summary>
    /// Phương pháp (reference method) từ Capability.xlsx. Idempotent theo tên hoặc id cố định. Mã PP-001… khớp form frontend.
    /// </summary>
    private async Task SyncReferenceMethodsFromCapabilityXlsxAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;
        var names = CapabilityReferenceMethodsSeedData.NameViList;

        for (var i = 0; i < names.Length; i++)
        {
            var nameVi = names[i];
            var code = $"PP-{i + 1:D3}";
            var nameKey = NormalizeSampleMatrixGroupKey(nameVi);
            var preferredId = StableGuid("vietlab:layer0:refmethod:" + nameKey);

            var byName = await db.ReferenceMethods.FirstOrDefaultAsync(
                r => r.NameVi != null && r.NameVi.Trim().ToLower() == nameVi.Trim().ToLower(),
                cancellationToken);

            if (byName is not null)
            {
                byName.NameVi = nameVi;
                byName.ReferenceMethodCode = code;
                byName.SequenceNumber = i + 1;
                byName.Status = "Active";
                byName.Notes = null;
                byName.NameEn = null;
                byName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: reference method by name — updated {Name}", nameVi);
                continue;
            }

            if (await db.ReferenceMethods.AnyAsync(r => r.ReferenceMethodId == preferredId, cancellationToken))
            {
                var row = await db.ReferenceMethods.FirstAsync(r => r.ReferenceMethodId == preferredId, cancellationToken);
                row.NameVi = nameVi;
                row.ReferenceMethodCode = code;
                row.SequenceNumber = i + 1;
                row.Status = "Active";
                row.Notes = null;
                row.NameEn = null;
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: reference method {Code} — updated", code);
                continue;
            }

            db.ReferenceMethods.Add(new ReferenceMethod
            {
                ReferenceMethodId = preferredId,
                ReferenceMethodCode = code,
                SequenceNumber = i + 1,
                NameVi = nameVi,
                NameEn = null,
                Status = "Active",
                Notes = null,
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted reference method {Code}", code);
        }
    }

    /// <summary>
    /// Loại thiết bị từ cột &quot;Thiết bị/ Equipment&quot; (Capability.xlsx). Mã TB-001… khớp form frontend.
    /// </summary>
    private async Task SyncEquipmentTypesFromCapabilityXlsxAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;
        var names = CapabilityEquipmentTypesSeedData.NameViList;

        for (var i = 0; i < names.Length; i++)
        {
            var nameVi = names[i];
            var code = $"TB-{i + 1:D3}";
            var nameKey = NormalizeSampleMatrixGroupKey(nameVi);
            var preferredId = StableGuid("vietlab:layer0:eqtype:" + nameKey);

            var byName = await db.EquipmentTypes.FirstOrDefaultAsync(
                e => e.NameVi != null && e.NameVi.Trim().ToLower() == nameVi.Trim().ToLower(),
                cancellationToken);

            if (byName is not null)
            {
                byName.NameVi = nameVi;
                byName.EquipmentTypeCode = code;
                byName.Status = "Active";
                byName.NameEn = null;
                byName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: equipment type by name — updated {Name}", nameVi);
                continue;
            }

            if (await db.EquipmentTypes.AnyAsync(e => e.EquipmentTypeId == preferredId, cancellationToken))
            {
                var row = await db.EquipmentTypes.FirstAsync(e => e.EquipmentTypeId == preferredId, cancellationToken);
                row.NameVi = nameVi;
                row.EquipmentTypeCode = code;
                row.Status = "Active";
                row.NameEn = null;
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: equipment type {Code} — updated", code);
                continue;
            }

            db.EquipmentTypes.Add(new EquipmentType
            {
                EquipmentTypeId = preferredId,
                EquipmentTypeCode = code,
                NameVi = nameVi,
                NameEn = null,
                Status = "Active",
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted equipment type {Code} — {Name}", code, nameVi);
        }
    }

    /// <summary>
    /// Nhóm chỉ tiêu từ Capability.xlsx (seed <see cref="CapabilityAnalysisGroupsSeedData"/>). Mã NCT-0001…; giá <c>Giá nhóm chuẩn_new</c> → <see cref="AnalysisGroup.WholeGroupStandardPrice"/>.
    /// </summary>
    private async Task SyncAnalysisGroupsFromCapabilityImportAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;

        // Cột analysis_group_code không nullable: đổi mã NCT-* sang mã tạm duy nhất trước khi gán lại (tránh trùng uq_analysis_group_code).
        var allGroups = await db.AnalysisGroups.ToListAsync(cancellationToken);
        foreach (var g in allGroups)
        {
            var c = g.AnalysisGroupCode;
            if (c != null && c.StartsWith("NCT-", StringComparison.OrdinalIgnoreCase))
            {
                g.AnalysisGroupCode = "~TMP~" + g.AnalysisGroupId.ToString("N");
                g.UpdatedAt = utc;
            }
        }

        // Flush mã tạm lên DB trước khi gán NCT-* (tránh thứ tự lệnh INSERT/UPDATE trong một batch gây trùng unique).
        await db.SaveChangesAsync(cancellationToken);

        var rows = CapabilityAnalysisGroupsSeedData.Rows;

        for (var i = 0; i < rows.Length; i++)
        {
            var (nameVi, nameEn, wholePrice) = rows[i];
            var code = $"NCT-{i + 1:D4}";
            var nameKey = NormalizeSampleMatrixGroupKey(nameVi);
            var preferredId = StableGuid("vietlab:layer0:agroup:" + nameKey);

            var byName = await FindAnalysisGroupForLayer0SeedAsync(db, nameVi, cancellationToken);

            if (byName is not null)
            {
                byName.NameVi = nameVi;
                byName.NameEn = nameEn;
                byName.AnalysisGroupCode = code;
                byName.WholeGroupStandardPrice = wholePrice;
                byName.Status = "Active";
                byName.UpdatedAt = utc;
                _logger.LogInformation("Layer0: analysis group by name {Name} — updated ({Code})", nameVi, code);
                continue;
            }

            if (await db.AnalysisGroups.AnyAsync(g => g.AnalysisGroupId == preferredId, cancellationToken))
            {
                var row = await db.AnalysisGroups.FirstAsync(g => g.AnalysisGroupId == preferredId, cancellationToken);
                row.NameVi = nameVi;
                row.NameEn = nameEn;
                row.AnalysisGroupCode = code;
                row.WholeGroupStandardPrice = wholePrice;
                row.Status = "Active";
                row.UpdatedAt = utc;
                _logger.LogInformation("Layer0: analysis group {Code} — updated", code);
                continue;
            }

            db.AnalysisGroups.Add(new AnalysisGroup
            {
                AnalysisGroupId = preferredId,
                AnalysisGroupCode = code,
                NameVi = nameVi,
                NameEn = nameEn,
                WholeGroupStandardPrice = wholePrice,
                Status = "Active",
                CreatedAt = utc
            });
            _logger.LogInformation("Layer0: inserted analysis group {Code} — {Name}", code, nameVi);
        }

        // Bản ghi không khớp seed (ví dụ AG-001 demo, GPCT-*, nhóm cũ) vẫn giữ ~TMP~ sau bước đổi mã — gán NCT-* tiếp theo.
        var trackedGroups = await db.AnalysisGroups.ToListAsync(cancellationToken);
        var maxNctSeq = 0;
        foreach (var g in trackedGroups)
        {
            var c = g.AnalysisGroupCode;
            if (c is null || !c.StartsWith("NCT-", StringComparison.OrdinalIgnoreCase))
                continue;
            if (c.Length > 4 && int.TryParse(c.AsSpan(4), System.Globalization.NumberStyles.None, null, out var n) && n > maxNctSeq)
                maxNctSeq = n;
        }

        foreach (var g in trackedGroups
                     .Where(x => x.AnalysisGroupCode != null && x.AnalysisGroupCode.StartsWith("~TMP~", StringComparison.Ordinal))
                     .OrderBy(x => x.NameVi ?? "", StringComparer.OrdinalIgnoreCase))
        {
            maxNctSeq++;
            var newCode = $"NCT-{maxNctSeq:D4}";
            g.AnalysisGroupCode = newCode;
            g.UpdatedAt = utc;
            _logger.LogInformation("Layer0: analysis group ~TMP~ cleared → {Code} ({Name})", newCode, g.NameVi);
        }
    }

    /// <summary>Theo tên chuẩn seed hoặc <see cref="CapabilityAnalysisGroupsSeedData.LegacyNameAliases"/>.</summary>
    private static async Task<AnalysisGroup?> FindAnalysisGroupForLayer0SeedAsync(
        ApplicationDbContext db,
        string nameVi,
        CancellationToken cancellationToken)
    {
        var n = nameVi.Trim().ToLower();
        var hit = await db.AnalysisGroups.FirstOrDefaultAsync(
            g => g.NameVi != null && g.NameVi.Trim().ToLower() == n,
            cancellationToken);
        if (hit is not null)
            return hit;

        foreach (var (canon, legacy) in CapabilityAnalysisGroupsSeedData.LegacyNameAliases)
        {
            if (!string.Equals(canon.Trim(), nameVi.Trim(), StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (var leg in legacy)
            {
                var ln = leg.Trim().ToLower();
                hit = await db.AnalysisGroups.FirstOrDefaultAsync(
                    g => g.NameVi != null && g.NameVi.Trim().ToLower() == ln,
                    cancellationToken);
                if (hit is not null)
                    return hit;
            }

            break;
        }

        return null;
    }

    /// <summary>
    /// Đổi mã chỉ định cũ DES-* sang mã chuẩn (ISO, CUC_BVTV, …) nếu bản ghi mới chưa tồn tại.
    /// </summary>
    private async Task NormalizeLegacyDesignationCodesAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var pairs = new (string Legacy, string NewCode, string NewName)[]
        {
            ("DES-ISO", CapabilityImportRules.DesignationCodes.Iso, "ISO"),
            ("DES-BVTV", CapabilityImportRules.DesignationCodes.CucBvtv, "Cục BVTV"),
            ("DES-MOIT", CapabilityImportRules.DesignationCodes.BoCongThuong, "Bộ Công thương"),
            ("DES-NAFI", CapabilityImportRules.DesignationCodes.Nafi, "Nafi"),
            ("DES-CHANNUOI", CapabilityImportRules.DesignationCodes.CucChanNuoi, "Cục chăn nuôi")
        };

        foreach (var (legacy, newCode, newName) in pairs)
        {
            var oldRow = await db.Designations.FirstOrDefaultAsync(d => d.DesignationCode == legacy, cancellationToken);
            if (oldRow is null)
                continue;

            var newRow = await db.Designations.FirstOrDefaultAsync(d => d.DesignationCode == newCode, cancellationToken);
            if (newRow is not null)
            {
                _logger.LogWarning(
                    "Layer0: designation legacy {Legacy} exists but {NewCode} also exists — skip rename; remove duplicate manually if needed",
                    legacy, newCode);
                continue;
            }

            oldRow.DesignationCode = newCode;
            oldRow.Name = newName;
            oldRow.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Layer0: renamed designation {Legacy} → {NewCode}", legacy, newCode);
        }
    }

    /// <summary>
    /// Chỉ định dùng cho <see cref="DepartmentAnalysisCapabilityDesignation"/> (ExpiredDate = giá trị ô Excel).
    /// HCM: ISO, Cục BVTV, Bộ Công thương, Nafi, Cục chăn nuôi. CT: ISO (và NĐ 107 trên capability).
    /// BL/CM: không có cột chỉ định trong spec — chỉ NĐ 107 trên capability.
    /// </summary>
    private async Task SyncDesignationsAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var seeds = new (string Code, int Seq, string Name, string SymbolCode, string Description, string? Note)[]
        {
            (
                CapabilityImportRules.DesignationCodes.Iso,
                1,
                "ISO",
                "a",
                "Chỉ tiêu được công nhận ISO/IEC 17025:2017 / Parameter is accredited to ISO/IEC 17025:2017.",
                "Excel (a). HCM + Cần Thơ → DepartmentAnalysisCapabilityDesignation.ExpiredDate theo chi nhánh."
            ),
            (
                CapabilityImportRules.DesignationCodes.CucBvtv,
                2,
                "Cục BVTV",
                "b",
                "Chỉ tiêu được chỉ định của Cục Trồng trọt và Bảo vệ thực vật / Parameter is designated by the Department of Crop Production and Plant Protection.",
                "Excel (b), chi nhánh HCM → DepartmentAnalysisCapabilityDesignation.ExpiredDate."
            ),
            (
                CapabilityImportRules.DesignationCodes.BoCongThuong,
                3,
                "Bộ Công thương",
                "e",
                "Chỉ tiêu được chỉ định của Bộ Công thương / Parameter designated by the Ministry of Industry and Trade.",
                "Excel (e), chi nhánh HCM → DepartmentAnalysisCapabilityDesignation.ExpiredDate."
            ),
            (
                CapabilityImportRules.DesignationCodes.Nafi,
                4,
                "Nafi",
                "d",
                "Chỉ tiêu được chỉ định của Cục Chất lượng, Chế biến và Phát triển thị trường / Parameter is designated by the National Authority for Agro-Forestry-Fishery Quality, Processing and Market Development.",
                "Excel (d), chi nhánh HCM → DepartmentAnalysisCapabilityDesignation.ExpiredDate."
            ),
            (
                CapabilityImportRules.DesignationCodes.CucChanNuoi,
                5,
                "Cục chăn nuôi",
                "c",
                "Chỉ tiêu được chỉ định của Cục Chăn nuôi và Thú y / Parameter is designated by the Department of Livestock Production and Veterinary.",
                "Excel (c), chi nhánh HCM → DepartmentAnalysisCapabilityDesignation.ExpiredDate."
            )
        };

        var utc = DateTime.UtcNow;

        foreach (var (code, seq, name, symbolCode, description, note) in seeds)
        {
            var existing = await db.Designations.FirstOrDefaultAsync(d => d.DesignationCode == code, cancellationToken);
            if (existing is null)
            {
                var id = StableGuid("vietlab:layer0:designation:" + code);
                if (await db.Designations.AnyAsync(d => d.DesignationId == id, cancellationToken))
                    id = Guid.NewGuid();

                db.Designations.Add(new Designation
                {
                    DesignationId = id,
                    DesignationCode = code,
                    SequenceNumber = seq,
                    Name = name,
                    SymbolCode = symbolCode,
                    Description = description,
                    Note = note,
                    Status = "Active",
                    CreatedAt = utc
                });
                _logger.LogInformation("Layer0: inserted designation {Code}", code);
            }
            else
            {
                existing.SequenceNumber = seq;
                existing.Name = name;
                existing.SymbolCode = symbolCode;
                existing.Description = description;
                existing.Note = note;
                existing.Status = "Active";
                existing.UpdatedAt = utc;
                _logger.LogInformation("Layer0: updated designation {Code}", code);
            }
        }
    }

    private async Task SyncLaboratoryTechniquesAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var byCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SAC_KY"] = "Sắc ký",
            ["CO_DIEN"] = "Cổ điển",
            ["QUANG_PHO"] = "Quang phổ",
            ["VI_SINH"] = "Vi sinh"
        };

        foreach (var (code, nameVi) in byCode)
        {
            var lt = await db.LaboratoryTechniques.FirstOrDefaultAsync(
                x => x.TechniqueCode == code, cancellationToken);
            if (lt is null)
            {
                _logger.LogWarning("Layer0: laboratory technique {Code} not found — skip", code);
                continue;
            }

            if (!string.Equals(lt.NameVi, nameVi, StringComparison.Ordinal))
            {
                lt.NameVi = nameVi;
                lt.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Layer0: updated laboratory technique {Code} display name", code);
            }
        }
    }

    /// <summary>
    /// Phòng ban khớp cột "Bộ phận phụ trách (Kỹ thuật)" / "Kỹ thuật" (Vi sinh, Sắc ký, …) — mỗi chi nhánh một bộ.
    /// Hậu tố site lấy theo BranchCode. DB thực tế thường: SG→HCM, CT, BL, CM; giữ BR-001…BR-006 cho seed/demo cũ.
    /// </summary>
    private async Task SyncLaboratoryDisciplineDepartmentsAsync(ApplicationDbContext db, CancellationToken cancellationToken)
    {
        var suffixByBranchCode = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["BR-001"] = "HN",
            ["BR-002"] = "HCM",
            ["SG"] = "HCM",
            ["BR-003"] = "DN",
            ["BR-004"] = "CT",
            ["CT"] = "CT",
            ["BR-005"] = "BL",
            ["BL"] = "BL",
            ["BR-006"] = "CM",
            ["CM"] = "CM"
        };

        var disciplines = new (string Suffix, string NameVi, string NameEn)[]
        {
            ("VSINH", "Vi sinh", "Microbiology"),
            ("SACKY", "Sắc ký", "Chromatography"),
            ("QUANGPHO", "Quang phổ", "Spectroscopy"),
            ("CODIEN", "Cổ điển", "Classical methods")
        };

        var branches = await db.Branches.ToListAsync(cancellationToken);

        foreach (var branch in branches)
        {
            if (string.IsNullOrEmpty(branch.BranchCode) ||
                !suffixByBranchCode.TryGetValue(branch.BranchCode, out var siteSuffix))
            {
                _logger.LogWarning("Layer0: branch {Id} has unknown BranchCode — skip lab departments", branch.BranchId);
                continue;
            }

            foreach (var (discSuffix, nameVi, nameEn) in disciplines)
            {
                var deptCode = $"DEP-{siteSuffix}-{discSuffix}";
                var existing = await db.Departments.FirstOrDefaultAsync(d => d.DepartmentCode == deptCode, cancellationToken);
                var nameFullVi = $"{nameVi} ({branch.NameVi ?? branch.BranchCode})";
                var nameFullEn = $"{nameEn} ({branch.NameEn ?? branch.BranchCode})";

                if (existing is null)
                {
                    var id = StableGuid("vietlab:layer0:department:" + deptCode);
                    if (await db.Departments.AnyAsync(d => d.DepartmentId == id, cancellationToken))
                        id = Guid.NewGuid();

                    db.Departments.Add(new Department
                    {
                        DepartmentId = id,
                        DepartmentCode = deptCode,
                        BranchId = branch.BranchId,
                        NameVi = nameFullVi,
                        NameEn = nameFullEn,
                        Notes = "Layer0: phòng kỹ thuật theo nhóm — import Capability",
                        Status = "Active"
                    });
                    _logger.LogInformation("Layer0: inserted department {Code}", deptCode);
                }
                else
                {
                    existing.BranchId = branch.BranchId;
                    existing.NameVi = nameFullVi;
                    existing.NameEn = nameFullEn;
                    existing.Status = "Active";
                    existing.Notes = "Layer0: phòng kỹ thuật theo nhóm — import Capability";
                    existing.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Layer0: updated department {Code}", deptCode);
                }
            }
        }
    }
}
