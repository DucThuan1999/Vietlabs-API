using Microsoft.Extensions.Options;
using VietLab.Configuration;
using VietLab.Data;
using VietLab.Models;
using VietLab.Models.DTOs;

namespace VietLab.Services;

public class AmisCallbackService : IAmisCallbackService
{
    private readonly ApplicationDbContext _db;
    private readonly AmisOptions _options;
    private readonly ILogger<AmisCallbackService> _logger;

    public AmisCallbackService(
        ApplicationDbContext db,
        IOptions<AmisOptions> options,
        ILogger<AmisCallbackService> logger)
    {
        _db = db;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AmisCallbackDataOutput> HandleCallbackAsync(
        AmisCallbackDataInput input,
        CancellationToken cancellationToken = default)
    {
        var output = new AmisCallbackDataOutput();

        try
        {
            if (string.IsNullOrWhiteSpace(_options.AppId))
            {
                output.Success = false;
                output.ErrorCode = "Configuration";
                output.ErrorMessage = "Amis:AppId chưa được cấu hình.";
                return output;
            }

            var isValid = AmisSignatureHelper.ValidateSignature(input.Data, input.Signature, _options.AppId);
            await SaveCallbackLogAsync(input, isValid, cancellationToken);

            if (!isValid)
            {
                output.Success = false;
                output.ErrorCode = "InvalidParam";
                output.ErrorMessage = "Signature invalid";
                return output;
            }

            _logger.LogInformation(
                "AMIS callback logged: data_type={DataType}, org_company_code={OrgCompanyCode}",
                input.DataType,
                input.OrgCompanyCode);

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AMIS callback handling failed");
            output.Success = false;
            output.ErrorCode = "Exception";
            output.ErrorMessage = ex.Message;
            return output;
        }
    }

    private async Task SaveCallbackLogAsync(
        AmisCallbackDataInput input,
        bool isSignatureValid,
        CancellationToken cancellationToken)
    {
        var log = new AmisCallbackLog
        {
            AmisCallbackLogId = Guid.NewGuid(),
            Success = input.Success,
            ErrorCode = input.ErrorCode,
            ErrorMessage = input.ErrorMessage,
            Signature = input.Signature,
            DataType = input.DataType,
            Data = input.Data,
            OrgCompanyCode = input.OrgCompanyCode,
            AppId = input.AppId,
            IsSignatureValid = isSignatureValid,
            ReceivedAt = DateTime.UtcNow,
        };

        _db.AmisCallbackLogs.Add(log);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
