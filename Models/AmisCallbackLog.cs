namespace VietLab.Models;

/// <summary>Lưu callback inbound từ AMIS/MISA để audit và xử lý sau.</summary>
public class AmisCallbackLog
{
    public Guid AmisCallbackLogId { get; set; }

    public bool Success { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Signature { get; set; }

    public int DataType { get; set; }

    public string? Data { get; set; }

    public string? OrgCompanyCode { get; set; }

    public string? AppId { get; set; }

    public bool IsSignatureValid { get; set; }

    public DateTime ReceivedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string? ProcessingError { get; set; }
}
