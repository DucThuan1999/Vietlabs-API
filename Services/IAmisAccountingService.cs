using VietLab.Models.DTOs;

namespace VietLab.Services;

public interface IAmisAccountingService
{
    Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>> CreateCustomerAsync(
        CreateAmisCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDto>>> GetCustomersAsync(
        AmisPagedQuery query,
        CancellationToken cancellationToken = default);

    Task<AmisOperationResult<IReadOnlyList<AmisAccountObjectDebtDto>>> GetCustomerDebtsAsync(
        AmisDebtQuery query,
        CancellationToken cancellationToken = default);
}
