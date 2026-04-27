namespace VietLab.Data;

/// <summary>
/// Nhóm chỉ tiêu distinct từ <c>data/Capability.xlsx</c> sheet <b>Vietlabs</b>:
/// cột <see cref="CapabilityImportRules.AnalysisGroupNameColumnVi"/> + <see cref="CapabilityImportRules.AnalysisGroupWholeGroupStandardColumnVi"/>.
/// Giá: mode theo nhóm khi có nhiều mức. Mã NCT-0001… theo thứ tự tên A→Z (ordinal ignore case).
/// </summary>
internal static class CapabilityAnalysisGroupsSeedData
{
    public readonly record struct Row(string NameVi, string NameEn, decimal? WholeGroupStandardPrice);

    /// <summary>Tên chuẩn Excel → tên cũ từng dùng trong DB/CSV (cùng nghĩa) để cập nhật bản ghi hiện có thay vì tạo trùng.</summary>
    public static readonly (string Canonical, string[] LegacyDbNames)[] LegacyNameAliases =
    [
        ("BETA-AGONISTS", ["β-AGONISTS"]),
        ("Beta-lactam (Penicillins)", ["β-LACTAM (PENICILLINS)"]),
        ("FLUOROQUINOLONES", ["FLUOROQUINOLNES"]),
        ("NITROFURAN METABOLITES", ["NITROFURANS METABIOLIZE"]),
        ("PES-LC/MS/MS", ["PES-LC/MS/MS-1"]),
        ("Heavy metal-TP-ICP/MS", ["HEAVY METAL-ICP/MS"]),
        ("Metal-TP-ICP/MS", ["METAL/ICP-MS"]),
    ];

    public static readonly Row[] Rows =
    [
        new("AMINO ACIDS", "AMINO ACIDS", 1600000m),
        new("AMPHENICOL", "AMPHENICOL", 600000m),
        new("ANTHELMINTICS", "ANTHELMINTICS", 1600000m),
        new("AVERMECTINS", "AVERMECTINS", 600000m),
        new("B-group vitamins", "B-group vitamins", null),
        new("BETA-AGONISTS", "BETA-AGONISTS", 800000m),
        new("Beta-lactam (Penicillins)", "Beta-lactam (Penicillins)", 1200000m),
        new("CORTICOSTEROIDS", "CORTICOSTEROIDS", 1600000m),
        new("FATTY ACIDS", "FATTY ACIDS", 1600000m),
        new("FLUOROQUINOLONES", "FLUOROQUINOLONES", 800000m),
        new("FOOD PRESERVATIVES", "FOOD PRESERVATIVES", null),
        new("Haloacetic acids", "Haloacetic acids", 800000m),
        new("Heavy metal-Mỹ phẩm-ICP/MS", "Heavy metal-Mỹ phẩm-ICP/MS", null),
        new("Heavy metal-PGTP-ICP/MS", "Heavy metal-PGTP-ICP/MS", null),
        new("Heavy metal-Sữa-ICP/MS", "Heavy metal-Sữa-ICP/MS", null),
        new("Heavy metal-TP-ICP/MS", "Heavy metal-TP-ICP/MS", null),
        new("Heavy metal-TPBVSK-ICP/MS", "Heavy metal-TPBVSK-ICP/MS", null),
        new("MELAMINE", "MELAMINE", null),
        new("Metal-TACN-ICP/MS", "Metal-TACN-ICP/MS", null),
        new("Metal-TP-ICP/MS", "Metal-TP-ICP/MS", null),
        new("Metal-TPBVSK-ICP/MS", "Metal-TPBVSK-ICP/MS", null),
        new("MG/LMG", "MG/LMG", 700000m),
        new("MYCOTOXINS", "MYCOTOXINS", null),
        new("Nhóm Carbamat", "Nhóm Carbamat", 1400000m),
        new("Nhóm Chlor hữu cơ", "Nhóm Chlor hữu cơ", 1400000m),
        new("Nhóm Cúc", "Nhóm Cúc", 1500000m),
        new("Nhóm Lân", "Nhóm Lân", 1400000m),
        new("Nhóm Triazole", "Nhóm Triazole", 1400000m),
        new("NITROFURAN METABOLITES", "NITROFURAN METABOLITES", 700000m),
        new("NITROFURANS", "NITROFURANS", 800000m),
        new("NITROIMIDAZOLES", "NITROIMIDAZOLES", 700000m),
        new("PCBs", "PCBs", 1500000m),
        new("PES-GC-MS/MS", "PES-GC-MS/MS", 1400000m),
        new("PES-LC-MS/MS", "PES-LC-MS/MS", 600000m),
        new("PES-LC/MS/MS", "PES-LC/MS/MS", 1400000m),
        new("PFAS", "PFAS", 2000000m),
        new("PHENOLS", "PHENOLS", 1600000m),
        new("PHENOXY ACID HERBICIDES", "PHENOXY ACID HERBICIDES", 800000m),
        new("POLAR PESTICIDES", "POLAR PESTICIDES", 1200000m),
        new("Polycyclic Aromatic Hydrocarbons (PAHs)", "Polycyclic Aromatic Hydrocarbons (PAHs)", 1600000m),
        new("SUGARS", "SUGARS", 1200000m),
        new("SULFONAMIDES", "SULFONAMIDES", 800000m),
        new("SYNTHETIC FOOD COLORANTS", "SYNTHETIC FOOD COLORANTS", null),
        new("TETRACYCLINES", "TETRACYCLINES", 600000m),
        new("VOCs", "VOCs", 800000m),
    ];
}
