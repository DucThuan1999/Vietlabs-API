using System.Text.Json.Serialization;

namespace VietLab.Models.DTOs;

/// <summary>Response wrapper chuẩn AMIS (PascalCase).</summary>
public class AmisApiResponse
{
    public bool Success { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Data { get; set; }

    public string? CustomData { get; set; }
}

public class AmisConnectTokenData
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("expired_time")]
    public DateTime? ExpiredTime { get; set; }

    [JsonPropertyName("expired_time_ticks")]
    public long? ExpiredTimeTicks { get; set; }

    [JsonPropertyName("tenant_code")]
    public string? TenantCode { get; set; }

    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }
}

/// <summary>Đối tượng (khách hàng) — dictionary_type / get data_type = 1.</summary>
public class AmisAccountObjectDto
{
    [JsonPropertyName("dictionary_type")]
    public int DictionaryType { get; set; }

    [JsonPropertyName("account_object_id")]
    public string? AccountObjectId { get; set; }

    [JsonPropertyName("account_object_code")]
    public string? AccountObjectCode { get; set; }

    [JsonPropertyName("account_object_name")]
    public string? AccountObjectName { get; set; }

    [JsonPropertyName("company_tax_code")]
    public string? CompanyTaxCode { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("is_customer")]
    public bool IsCustomer { get; set; } = true;

    [JsonPropertyName("is_vendor")]
    public bool IsVendor { get; set; }

    [JsonPropertyName("is_employee")]
    public bool IsEmployee { get; set; }

    [JsonPropertyName("inactive")]
    public bool Inactive { get; set; }

    [JsonPropertyName("maximize_debt_amount")]
    public decimal? MaximizeDebtAmount { get; set; }

    [JsonPropertyName("receiptable_debt_amount")]
    public decimal? ReceiptableDebtAmount { get; set; }

    [JsonPropertyName("branch_id")]
    public string? BranchId { get; set; }
}

public class AmisAccountObjectDebtDto
{
    [JsonPropertyName("account_object_id")]
    public string? AccountObjectId { get; set; }

    [JsonPropertyName("account_object_code")]
    public string? AccountObjectCode { get; set; }

    [JsonPropertyName("account_object_name")]
    public string? AccountObjectName { get; set; }

    [JsonPropertyName("organization_unit_id")]
    public string? OrganizationUnitId { get; set; }

    [JsonPropertyName("organization_unit_code")]
    public string? OrganizationUnitCode { get; set; }

    [JsonPropertyName("organization_unit_name")]
    public string? OrganizationUnitName { get; set; }

    [JsonPropertyName("debt_amount")]
    public decimal DebtAmount { get; set; }

    [JsonPropertyName("invoice_debt_amount")]
    public decimal InvoiceDebtAmount { get; set; }
}

public class AmisCustomDataLastSync
{
    [JsonPropertyName("LastSyncTime")]
    public DateTime? LastSyncTime { get; set; }
}

/// <summary>Body tạo khách hàng qua API nội bộ VietLab.</summary>
public class CreateAmisCustomerRequest
{
    /// <summary>Nếu có, map từ bản ghi Client trong CRM.</summary>
    public Guid? ClientId { get; set; }

    public string? AccountObjectId { get; set; }

    public string? AccountObjectCode { get; set; }

    public string? AccountObjectName { get; set; }

    public string? CompanyTaxCode { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string? BranchId { get; set; }

    public bool IsCustomer { get; set; } = true;

    public bool IsVendor { get; set; }

    public decimal? MaximizeDebtAmount { get; set; }

    public decimal? ReceiptableDebtAmount { get; set; }
}

public class AmisPagedQuery
{
    public int Skip { get; set; }

    public int Take { get; set; } = 100;

    public string? LastSyncTime { get; set; }

    public string? BranchId { get; set; }
}

public class AmisDebtQuery : AmisPagedQuery
{
    /// <summary>0 = phải thu, 1 = phải trả.</summary>
    public int DataType { get; set; }
}

public class AmisOperationResult<T>
{
    public bool Success { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public T? Data { get; set; }

    public string? CustomData { get; set; }
}
