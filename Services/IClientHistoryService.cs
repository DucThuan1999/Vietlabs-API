using VietLab.Models;

namespace VietLab.Services;

public interface IClientHistoryService
{
    Task LogClientChangeAsync(Guid clientId, string changeDescription, Guid changedByAccountId, string? changeType = null);
    Task<List<ClientHistory>> GetClientHistoryAsync(Guid clientId);
}

