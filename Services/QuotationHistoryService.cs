using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Services;

public class QuotationHistoryService : IQuotationHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuotationHistoryService> _logger;

    public QuotationHistoryService(
        ApplicationDbContext context,
        ILogger<QuotationHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogQuotationChangeAsync(
        Guid quotationId, 
        string changeDescription, 
        Guid changedByAccountId, 
        string? changeType = null,
        string? oldValues = null,
        string? newValues = null)
    {
        try
        {
            var history = new QuotationHistory
            {
                QuotationHistoryId = Guid.NewGuid(),
                QuotationId = quotationId,
                ChangedDate = DateTime.UtcNow,
                ChangeDescription = changeDescription,
                ChangedByAccountId = changedByAccountId,
                ChangeType = changeType ?? "Updated",
                OldValues = oldValues,
                NewValues = newValues
            };

            _context.QuotationHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quotation history logged: QuotationId={QuotationId}, ChangeType={ChangeType}, ChangedBy={AccountId}",
                quotationId, changeType, changedByAccountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging quotation history: QuotationId={QuotationId}, Error={Error}",
                quotationId, ex.Message);
            // Không throw exception để không làm gián đoạn flow chính
        }
    }

    public async Task<List<QuotationHistory>> GetQuotationHistoryAsync(Guid quotationId)
    {
        return await _context.QuotationHistories
            .Include(qh => qh.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .Where(qh => qh.QuotationId == quotationId)
            .OrderByDescending(qh => qh.ChangedDate)
            .ToListAsync();
    }
}

