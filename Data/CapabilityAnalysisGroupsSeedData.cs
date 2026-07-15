namespace VietLab.Data;

/// <summary>
/// Nhóm chỉ tiêu distinct từ Danh mục Năng lực v2 sheet Vietlabs:
/// cột Nhóm Chỉ tiêu + Giá nhóm chuẩn_new (mode theo nhóm).
/// </summary>
internal static class CapabilityAnalysisGroupsSeedData
{
    public readonly record struct Row(string NameVi, string NameEn, decimal? WholeGroupStandardPrice);

    /// <summary>Tên chuẩn seed → tên cũ trong DB (khác hoa thường / đổ tên).</summary>
    public static readonly (string Canon, string[] Legacy)[] LegacyNameAliases =
    [
        ("Nhóm Lân", ["nhóm Lân"]),
    ];

    public static readonly Row[] Rows =
    [
        new("AMINO ACIDS", "AMINO ACIDS", 1600000m),
        new("AMPHENICOL", "AMPHENICOL", 600000m),
        new("ANTHELMINTICS", "ANTHELMINTICS", 1600000m),
        new("AVERMECTINS", "AVERMECTINS", 600000m),
        new("BETA-AGONISTS", "BETA-AGONISTS", 800000m),
        new("Beta-lactam (Penicillins)", "Beta-lactam (Penicillins)", 1200000m),
        new("FATTY ACIDS", "FATTY ACIDS", 1600000m),
        new("FLUOROQUINOLONES", "FLUOROQUINOLONES", 800000m),
        new("Haloacetic acids", "Haloacetic acids", 800000m),
        new("MG/LMG", "MG/LMG", 700000m),
        new("Nhóm Carbamat", "Nhóm Carbamat", 1600000m),
        new("Nhóm Chlor hữu cơ", "Nhóm Chlor hữu cơ", 1400000m),
        new("Nhóm Cúc", "Nhóm Cúc", 1500000m),
        new("Nhóm Lân", "Nhóm Lân", 1600000m),
        new("nhóm Lân", "nhóm Lân", 1400000m),
        new("Nhóm Triazole", "Nhóm Triazole", 1400000m),
        new("NITROFURAN METABOLITES", "NITROFURAN METABOLITES", 700000m),
        new("NITROFURANS", "NITROFURANS", 800000m),
        new("NITROIMIDAZOLES", "NITROIMIDAZOLES", 700000m),
        new("PCBs", "PCBs", 1500000m),
        new("PES-GC-MS/MS", "PES-GC-MS/MS", 1400000m),
        new("PES-LC-MS/MS", "PES-LC-MS/MS", 1600000m),
        new("PFAS", "PFAS", 2000000m),
        new("PHENOLS", "PHENOLS", 1600000m),
        new("PHENOXY ACID HERBICIDES", "PHENOXY ACID HERBICIDES", 800000m),
        new("POLAR PESTICIDES", "POLAR PESTICIDES", 1200000m),
        new("Polycyclic Aromatic Hydrocarbons (PAHs)", "Polycyclic Aromatic Hydrocarbons (PAHs)", 1600000m),
        new("SUGARS", "SUGARS", 1200000m),
        new("SULFONAMIDES", "SULFONAMIDES", 800000m),
        new("TETRACYCLINES", "TETRACYCLINES", 600000m),
        new("VOCs", "VOCs", 800000m),
    ];
}
