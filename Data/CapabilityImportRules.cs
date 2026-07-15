using System.Collections.ObjectModel;

namespace VietLab.Data;

/// <summary>
/// Một dòng nền mẫu từ Capability.xlsx: nhóm (cột Nhóm nền mẫu) + nền mẫu (cột Nền mẫu) + mẫu đã đăng ký nếu có.
/// </summary>
public readonly record struct SampleMatrixSeed(string GroupNameVi, string MatrixNameVi, string? RegisteredMatrix);

/// <summary>
/// Quy tắc map sheet Capability (Vietlabs) → <see cref="VietLab.Models.DepartmentAnalysisCapability"/>
/// và <see cref="VietLab.Models.DepartmentAnalysisCapabilityDesignation"/>.
/// Danh mục <see cref="VietLab.Models.Designation"/> được seed bởi <see cref="Layer0ReferenceDataSeeder"/> (mã <see cref="DesignationCodes"/>).
/// </summary>
/// <remarks>
/// Chi nhánh: dùng đúng bản ghi chi nhánh đã có trong DB (không seed thêm trong layer 0).
/// <list type="bullet">
/// <item><b>HCM</b> — mã chi nhánh thực tế thường <c>SG</c> (legacy demo <c>BR-002</c>): cột Năng lực HCM (NĐ 107) → <c>Nd107</c>/<c>Nd107ExpiredDate</c> (ô <c>Chưa có</c> → <c>Nd107 = false</c>).
/// Các cột chỉ định → <c>DepartmentAnalysisCapabilityDesignation.ExpiredDate</c> (giá trị ô = ngày; <c>Chưa có</c>/<c>x</c> xử lý ở bước import).</item>
/// <item><b>Cần Thơ</b> — mã chi nhánh thường <c>CT</c> (legacy <c>BR-004</c>): Năng lực CT (NĐ 107) → cùng quy tắc NĐ 107; cột ISO (a) → chỉ định ISO.</item>
/// <item><b>Bạc Liêu</b>: chỉ cột Năng lực BL (NĐ 107) → <c>Nd107</c>/<c>Nd107ExpiredDate</c> (không có cột chỉ định riêng trên spec).</item>
/// <item><b>Cà Mau</b>: chỉ cột Năng lực CM (NĐ 107) → tương tự BL.</item>
/// </list>
/// </remarks>
public static class CapabilityImportRules
{
    /// <summary>Mã chi nhánh HCM trên DB thực tế (import / năng lực). Seed demo có thể dùng BR-002.</summary>
    public const string DefaultBranchCodeHcm = "SG";

    public const string DefaultBranchCodeCanTho = "CT";

    /// <summary>Mã chi nhánh Bạc Liêu — thường <c>BL</c> (legacy <c>BR-005</c>).</summary>
    public const string DefaultBranchCodeBacLieu = "BL";

    /// <summary>Mã chi nhánh Cà Mau — thường <c>CM</c> (legacy <c>BR-006</c>).</summary>
    public const string DefaultBranchCodeCaMau = "CM";

    /// <summary>Mã <see cref="VietLab.Models.Designation.DesignationCode"/> sau khi seed layer 0 (dùng khi tra cứu FK).</summary>
    public static class DesignationCodes
    {
        public const string Iso = "ISO";
        public const string CucBvtv = "CUC_BVTV";
        public const string BoCongThuong = "BO_CONG_THUONG";
        public const string Nafi = "NAFI";
        public const string CucChanNuoi = "CUC_CHAN_NUOI";
    }

    /// <summary>
    /// Tên cột trên Capability.xlsx (sheet Vietlabs; hoặc CSV export) map sang <c>analysis_group.whole_group_standard_price</c>.
    /// </summary>
    public const string AnalysisGroupWholeGroupStandardColumnVi = "Giá nhóm chuẩn_new";

    /// <summary>Tên cột legacy (export CSV cũ) — cùng ý nghĩa với <see cref="AnalysisGroupWholeGroupStandardColumnVi"/>.</summary>
    public const string AnalysisGroupWholeGroupStandardColumnLegacyEn = "Analysis Group Whole group standard";

    /// <summary>Tên cột nhóm chỉ tiêu trên Capability.xlsx (sheet <b>Vietlabs</b> và <b>NTP</b>) — trong file gốc là <c>Nhóm Chỉ tiêu</c>.</summary>
    public const string AnalysisGroupNameColumnVi = "Nhóm Chỉ tiêu";

    /// <summary>
    /// Một dòng seed nhóm nền mẫu (tên VN từ Excel + bản dịch EN chuẩn hóa).
    /// </summary>
    public readonly record struct SampleMatrixGroupSeed(string NameVi, string NameEn);

    /// <summary>
    /// Distinct cột &quot;Nhóm nền mẫu&quot; từ sheet <b>Vietlabs</b> và <b>NTP</b> của Capability.xlsx
    /// (đã gộp trùng không phân biệt hoa thường, ví dụ hai biến thể &quot;Bao bì… Thực phẩm/thực phẩm&quot;).
    /// Seed vào <see cref="VietLab.Models.SampleMatrixGroup"/> qua <see cref="Layer0ReferenceDataSeeder"/>.
    /// </summary>
        public static readonly ReadOnlyCollection<SampleMatrixGroupSeed> SampleMatrixGroupsFromCapabilityXlsx =
        new(
        [
            new("Bao bì tiếp xúc với thực phẩm", "Food contact packaging"),
            new("Dầu mỡ động thực vật", "Animal and vegetable oils and fats"),
            new("Hóa chất", "Chemicals"),
            new("Mẫu vệ sinh bề mặt", "Surface hygiene samples"),
            new("Mỹ phẩm", "Cosmetics"),
            new("Nước", "Water"),
            new("Nước uống có cồn và không cồn", "Alcoholic and non-alcoholic beverages"),
            new("Phân bón, chế phẩm sinh học", "Fertilizers and biological products"),
            new("phụ gia Thực phẩm", "Food additives"),
            new("Thức ăn và nguyên liệu thức ăn", "Animal feed and feed ingredients"),
            new("Thực phẩm", "Food"),
            new("Đất", "Soil"),
        ]);

    /// <summary>
    /// Distinct cặp (Nhóm nền mẫu, Nền mẫu) từ sheet Vietlabs + NTP; dữ liệu trong <see cref="CapabilitySampleMatricesSeedData"/>.
    /// </summary>
    public static IReadOnlyList<SampleMatrixSeed> SampleMatricesFromCapabilityXlsx => CapabilitySampleMatricesSeedData.Rows;

    /// <summary>
    /// Distinct cột &quot;Quy chuẩn/Tiêu chuẩn&quot; từ sheet Vietlabs + NTP — danh sách trong <see cref="CapabilityStandardsSeedData"/>.
    /// </summary>
    public static IReadOnlyList<string> StandardNamesFromCapabilityXlsx => CapabilityStandardsSeedData.NameViList;

    /// <summary>
    /// Distinct cột &quot;Phương pháp&quot; từ sheet Vietlabs + NTP — danh sách trong <see cref="CapabilityReferenceMethodsSeedData"/>.
    /// </summary>
    public static IReadOnlyList<string> ReferenceMethodNamesFromCapabilityXlsx =>
        CapabilityReferenceMethodsSeedData.NameViList;

    /// <summary>
    /// Distinct cột &quot;Thiết bị/ Equipment&quot; từ sheet Vietlabs + NTP — danh sách trong <see cref="CapabilityEquipmentTypesSeedData"/>.
    /// </summary>
    public static IReadOnlyList<string> EquipmentTypeNamesFromCapabilityXlsx =>
        CapabilityEquipmentTypesSeedData.NameViList;
}
