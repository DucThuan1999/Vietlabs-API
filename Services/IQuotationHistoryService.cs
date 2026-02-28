using VietLab.Models;

namespace VietLab.Services;

public interface IQuotationHistoryService
{
    Task LogQuotationChangeAsync(
        Guid quotationId, 
        string changeDescription, 
        Guid changedByAccountId, 
        string? changeType = null,
        string? oldValues = null,
        string? newValues = null);
    
    Task<List<QuotationHistory>> GetQuotationHistoryAsync(Guid quotationId);
}

