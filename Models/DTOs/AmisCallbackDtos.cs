using System.Text.Json.Serialization;

namespace VietLab.Models.DTOs;

/// <summary>Loại dữ liệu callback AMIS ACT Open.</summary>
public enum AmisCallbackDataType
{
    None = 0,
    SaveVoucher = 1,
    DeleteVoucher = 2,
    UpdateDocumentRef = 4,
    UpdateTaxInfoAsp = 5,
}

/// <summary>Payload callback từ AMIS/MISA (snake_case).</summary>
public class AmisCallbackDataInput
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    [JsonPropertyName("data_type")]
    public int DataType { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("org_company_code")]
    public string? OrgCompanyCode { get; set; }

    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }
}

/// <summary>Response callback trả về AMIS (PascalCase theo mẫu MISA).</summary>
public class AmisCallbackDataOutput
{
    [JsonPropertyName("Success")]
    public bool Success { get; set; } = true;

    [JsonPropertyName("ErrorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("ErrorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonPropertyName("Data")]
    public string? Data { get; set; }
}

/// <summary>Chi tiết trong trường data khi data_type = SaveVoucher / DeleteVoucher.</summary>
public class AmisCallbackDataDetail
{
    [JsonPropertyName("org_refid")]
    public string? OrgRefId { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonPropertyName("session_id")]
    public Guid? SessionId { get; set; }

    [JsonPropertyName("error_call_back_message")]
    public string? ErrorCallBackMessage { get; set; }

    [JsonPropertyName("voucher_type")]
    public int? VoucherType { get; set; }
}
