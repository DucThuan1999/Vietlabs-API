using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VietLab.Configuration;
using VietLab.Data;
using VietLab.Models.DTOs;

namespace VietLab.Services;

public class AmisAccountingService : IAmisAccountingService
{
    public const int AccountObjectDictionaryType = 1;
    public const int DebtReceivableDataType = 0;
    public const int DebtPayableDataType = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null,
    };

    private static readonly JsonSerializerOptions JsonCamelOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _httpClient;
    private readonly AmisOptions _options;
    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AmisAccountingService> _logger;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    public AmisAccountingService(
        HttpClient httpClient,
        IOptions<AmisOptions> options,
        ApplicationDbContext db,
        IMemoryCache cache,
        ILogger<AmisAccountingService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public async Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>> CreateCustomerAsync(
        CreateAmisCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var configError = ValidateConfig();
        if (configError != null)
            return FailList<AmisAccountObjectDto>(configError.Value.Code, configError.Value.Message);

        var accountObject = await BuildAccountObjectAsync(request, cancellationToken);
        if (accountObject == null)
        {
            return FailList<AmisAccountObjectDto>("InvalidParam", "Không đủ dữ liệu để tạo khách hàng (account_object_code, account_object_name).");
        }

        var body = new
        {
            app_id = _options.AppId,
            org_company_code = _options.OrgCompanyCode,
            dictionary = new[] { accountObject },
        };

        var response = await PostAmisAsync("/apir/sync/actopen/save_dictionary", body, cancellationToken);
        return await MapDictionaryResponseAsync(response);
    }

    public async Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>> GetCustomersAsync(
        AmisPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var configError = ValidateConfig();
        if (configError != null)
            return FailList<AmisAccountObjectDto>(configError.Value.Code, configError.Value.Message);

        var take = Math.Clamp(query.Take <= 0 ? _options.DefaultTake : query.Take, 1, 100);
        var body = new
        {
            app_id = _options.AppId,
            org_company_code = _options.OrgCompanyCode,
            data_type = AccountObjectDictionaryType,
            branch_id = query.BranchId,
            skip = query.Skip,
            take,
            last_sync_time = query.LastSyncTime,
        };

        var response = await PostAmisAsync("/apir/sync/actopen/get_dictionary", body, cancellationToken);
        return await MapDictionaryResponseAsync(response);
    }

    public async Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDebtDto>>> GetCustomerDebtsAsync(
        AmisDebtQuery query,
        CancellationToken cancellationToken = default)
    {
        var configError = ValidateConfig();
        if (configError != null)
            return FailList<AmisAccountObjectDebtDto>(configError.Value.Code, configError.Value.Message);

        var take = Math.Clamp(query.Take <= 0 ? _options.DefaultTake : query.Take, 1, 100);
        var body = new
        {
            app_id = _options.AppId,
            org_company_code = _options.OrgCompanyCode,
            data_type = query.DataType,
            branch_id = query.BranchId,
            skip = query.Skip,
            take,
            last_sync_time = query.LastSyncTime,
        };

        var response = await PostAmisAsync("/apir/sync/actopen/get_list_acc_obj_debt", body, cancellationToken);
        if (!response.Success)
        {
            return new AmisOperationResult<IReadOnlyList<AmisAccountObjectDebtDto>>
            {
                Success = false,
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.ErrorMessage,
            };
        }

        var items = DeserializeDataList<AmisAccountObjectDebtDto>(response.Data);
        return new AmisOperationResult<IReadOnlyList<AmisAccountObjectDebtDto>>
        {
            Success = true,
            Data = items,
            CustomData = response.CustomData,
        };
    }

    private (string Code, string Message)? ValidateConfig()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiUrl))
            return ("Configuration", "Amis:ApiUrl chưa được cấu hình.");
        if (string.IsNullOrWhiteSpace(_options.AppId))
            return ("Configuration", "Amis:AppId chưa được cấu hình.");
        if (string.IsNullOrWhiteSpace(_options.AccessCode))
            return ("Configuration", "Amis:AccessCode chưa được cấu hình.");
        if (string.IsNullOrWhiteSpace(_options.OrgCompanyCode))
            return ("Configuration", "Amis:OrgCompanyCode chưa được cấu hình.");
        return null;
    }

    private static AmisOperationResult<IReadOnlyList<T>> FailList<T>(string code, string message) =>
        new() { Success = false, ErrorCode = code, ErrorMessage = message };

    private async Task<AmisAccountObjectDto?> BuildAccountObjectAsync(
        CreateAmisCustomerRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ClientId.HasValue && request.ClientId.Value != Guid.Empty)
        {
            var client = await _db.Clients.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == request.ClientId.Value, cancellationToken);
            if (client == null)
                return null;

            return new AmisAccountObjectDto
            {
                DictionaryType = AccountObjectDictionaryType,
                AccountObjectId = request.AccountObjectId ?? client.ClientId.ToString(),
                AccountObjectCode = request.AccountObjectCode ?? client.InternalCode ?? client.ClientId.ToString(),
                AccountObjectName = request.AccountObjectName ?? client.CompanyName,
                CompanyTaxCode = request.CompanyTaxCode ?? client.TaxCode,
                Address = request.Address ?? client.Address,
                Country = request.Country ?? client.Country ?? "Việt Nam",
                IsCustomer = request.IsCustomer,
                IsVendor = request.IsVendor,
                MaximizeDebtAmount = request.MaximizeDebtAmount,
                ReceiptableDebtAmount = request.ReceiptableDebtAmount,
                BranchId = request.BranchId,
                Inactive = false,
            };
        }

        if (string.IsNullOrWhiteSpace(request.AccountObjectCode) || string.IsNullOrWhiteSpace(request.AccountObjectName))
            return null;

        return new AmisAccountObjectDto
        {
            DictionaryType = AccountObjectDictionaryType,
            AccountObjectId = request.AccountObjectId ?? Guid.NewGuid().ToString(),
            AccountObjectCode = request.AccountObjectCode.Trim(),
            AccountObjectName = request.AccountObjectName.Trim(),
            CompanyTaxCode = request.CompanyTaxCode,
            Address = request.Address,
            Country = request.Country ?? "Việt Nam",
            IsCustomer = request.IsCustomer,
            IsVendor = request.IsVendor,
            MaximizeDebtAmount = request.MaximizeDebtAmount,
            ReceiptableDebtAmount = request.ReceiptableDebtAmount,
            BranchId = request.BranchId,
            Inactive = false,
        };
    }

    private async Task<AmisApiResponse> PostAmisAsync(string relativePath, object body, CancellationToken cancellationToken)
    {
        var token = await GetAccessTokenAsync(cancellationToken);
        var url = CombineUrl(_options.ApiUrl, relativePath);
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("X-MISA-AccessToken", token);
        request.Content = new StringContent(
            JsonSerializer.Serialize(body, JsonCamelOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("AMIS HTTP {StatusCode} {Path}: {Body}", (int)response.StatusCode, relativePath, content);
            return new AmisApiResponse
            {
                Success = false,
                ErrorCode = "HttpError",
                ErrorMessage = $"AMIS HTTP {(int)response.StatusCode}: {content}",
            };
        }

        var parsed = JsonSerializer.Deserialize<AmisApiResponse>(content, JsonOptions);
        if (parsed == null)
        {
            return new AmisApiResponse
            {
                Success = false,
                ErrorCode = "ParseError",
                ErrorMessage = "Không parse được response AMIS.",
            };
        }

        if (!parsed.Success && string.Equals(parsed.ErrorCode, "ExpiredToken", StringComparison.OrdinalIgnoreCase))
        {
            _cache.Remove(CacheKey());
            token = await GetAccessTokenAsync(cancellationToken);
            using var retryRequest = new HttpRequestMessage(HttpMethod.Post, url);
            retryRequest.Headers.Add("X-MISA-AccessToken", token);
            retryRequest.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonCamelOptions),
                Encoding.UTF8,
                "application/json");
            using var retryResponse = await _httpClient.SendAsync(retryRequest, cancellationToken);
            var retryContent = await retryResponse.Content.ReadAsStringAsync(cancellationToken);
            parsed = JsonSerializer.Deserialize<AmisApiResponse>(retryContent, JsonOptions)
                    ?? parsed;
        }

        return parsed;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKey();
        if (_cache.TryGetValue<AmisConnectTokenData>(cacheKey, out var cached) &&
            cached?.AccessToken != null &&
            !IsTokenExpired(cached))
        {
            return cached.AccessToken;
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue<AmisConnectTokenData>(cacheKey, out cached) &&
                cached?.AccessToken != null &&
                !IsTokenExpired(cached))
            {
                return cached.AccessToken;
            }

            var connectBody = new
            {
                app_id = _options.AppId,
                access_code = _options.AccessCode,
                org_company_code = _options.OrgCompanyCode,
            };

            var url = CombineUrl(_options.ApiUrl, "/api/oauth/actopen/connect");
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(
                JsonSerializer.Serialize(connectBody, JsonCamelOptions),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"AMIS connect failed HTTP {(int)response.StatusCode}: {content}");
            }

            var apiResponse = JsonSerializer.Deserialize<AmisApiResponse>(content, JsonOptions);
            if (apiResponse == null || !apiResponse.Success || string.IsNullOrEmpty(apiResponse.Data))
            {
                throw new InvalidOperationException(
                    $"AMIS connect failed: {apiResponse?.ErrorCode} {apiResponse?.ErrorMessage}");
            }

            var tokenData = JsonSerializer.Deserialize<AmisConnectTokenData>(apiResponse.Data, JsonOptions);
            if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
            {
                throw new InvalidOperationException("AMIS connect: không lấy được access_token.");
            }

            var expiry = tokenData.ExpiredTime ?? DateTime.UtcNow.AddHours(11);
            var cacheDuration = expiry - DateTime.UtcNow - TimeSpan.FromMinutes(_options.TokenRefreshBeforeExpiryMinutes);
            if (cacheDuration < TimeSpan.FromMinutes(1))
                cacheDuration = TimeSpan.FromMinutes(1);

            _cache.Set(cacheKey, tokenData, cacheDuration);
            return tokenData.AccessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private bool IsTokenExpired(AmisConnectTokenData token)
    {
        if (token.ExpiredTimeTicks.HasValue && token.ExpiredTimeTicks.Value > 0)
        {
            var expiry = new DateTime(token.ExpiredTimeTicks.Value, DateTimeKind.Utc);
            return DateTime.UtcNow >= expiry.AddMinutes(-_options.TokenRefreshBeforeExpiryMinutes);
        }

        if (token.ExpiredTime.HasValue)
        {
            return DateTime.UtcNow >= token.ExpiredTime.Value.ToUniversalTime()
                .AddMinutes(-_options.TokenRefreshBeforeExpiryMinutes);
        }

        return false;
    }

    private static string CacheKey() => "amis:access_token";

    private static string CombineUrl(string baseUrl, string path)
    {
        var b = baseUrl.TrimEnd('/');
        var p = path.StartsWith('/') ? path : "/" + path;
        return b + p;
    }

    private static Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>> MapDictionaryResponseAsync(
        AmisApiResponse response)
    {
        if (!response.Success)
        {
            return Task.FromResult(new AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>
            {
                Success = false,
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.ErrorMessage,
            });
        }

        var items = DeserializeDataList<AmisAccountObjectDto>(response.Data);
        return Task.FromResult(new AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>
        {
            Success = true,
            Data = items,
            CustomData = response.CustomData,
        });
    }

    private static IReadOnlyList<T> DeserializeDataList<T>(string? dataJson)
    {
        if (string.IsNullOrWhiteSpace(dataJson))
            return Array.Empty<T>();

        try
        {
            return JsonSerializer.Deserialize<List<T>>(dataJson, JsonOptions) ?? new List<T>();
        }
        catch
        {
            return Array.Empty<T>();
        }
    }
}
