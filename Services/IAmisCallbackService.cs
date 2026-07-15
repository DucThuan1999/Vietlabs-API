using VietLab.Models.DTOs;

namespace VietLab.Services;

public interface IAmisCallbackService
{
    Task<AmisCallbackDataOutput> HandleCallbackAsync(
        AmisCallbackDataInput input,
        CancellationToken cancellationToken = default);
}
