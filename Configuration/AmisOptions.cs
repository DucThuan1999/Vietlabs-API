namespace VietLab.Configuration;

/// <summary>
/// Cấu hình gọi API AMIS/MISA ACT Open (outbound).
/// </summary>
public class AmisOptions
{
    public const string SectionName = "Amis";

    /// <summary>Base URL AMIS, ví dụ https://actapp.misa.vn</summary>
    public string ApiUrl { get; set; } = "";

    public string AppId { get; set; } = "";

    /// <summary>Mã kết nối công ty (tab API kết nối trên AMIS).</summary>
    public string AccessCode { get; set; } = "";

    /// <summary>Domain đối tác trên AMIS.</summary>
    public string OrgCompanyCode { get; set; } = "";

    /// <summary>Số bản ghi mặc định mỗi lần get (tối đa 100 theo tài liệu MISA).</summary>
    public int DefaultTake { get; set; } = 100;

    /// <summary>Refresh token trước khi hết hạn (phút). Token AMIS ~12h.</summary>
    public int TokenRefreshBeforeExpiryMinutes { get; set; } = 30;
}
