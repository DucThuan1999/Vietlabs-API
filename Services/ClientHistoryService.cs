using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Services;

public class ClientHistoryService : IClientHistoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClientHistoryService> _logger;

    public ClientHistoryService(
        ApplicationDbContext context,
        ILogger<ClientHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogClientChangeAsync(Guid clientId, string changeDescription, Guid changedByAccountId, string? changeType = null)
    {
        try
        {
            var history = new ClientHistory
            {
                ClientHistoryId = Guid.NewGuid(),
                ClientId = clientId,
                ChangedDate = DateTime.UtcNow,
                ChangeDescription = changeDescription,
                ChangedByAccountId = changedByAccountId,
                ChangeType = changeType ?? "Updated"
            };

            _context.ClientHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Client history logged: ClientId={ClientId}, ChangeType={ChangeType}, ChangedBy={AccountId}",
                clientId, changeType, changedByAccountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging client history: ClientId={ClientId}, Error={Error}",
                clientId, ex.Message);
            // Không throw exception để không làm gián đoạn flow chính
        }
    }

    public async Task<List<ClientHistory>> GetClientHistoryAsync(Guid clientId)
    {
        return await _context.ClientHistories
            .Include(ch => ch.ChangedByAccount)
                .ThenInclude(a => a!.Employee)
            .Where(ch => ch.ClientId == clientId)
            .OrderByDescending(ch => ch.ChangedDate)
            .ToListAsync();
    }
}

