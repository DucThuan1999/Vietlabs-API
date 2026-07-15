using Microsoft.EntityFrameworkCore;
using VietLab.Data;

namespace VietLab.Helpers;

public static class RegistrationPermitLabelReader
{
    public const string DefaultDisplayName = "NĐ 22/2026";

    public static readonly Guid SingletonId = Guid.Parse("0891de10-6c6a-4b54-8373-3fd73ef4ac0c");

    public static async Task<string> GetDisplayNameAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        var row = await context.RegistrationPermitLabelConfigs
            .AsNoTracking()
            .Where(x => x.RegistrationPermitLabelConfigId == SingletonId)
            .Select(x => x.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        return NormalizeDisplayName(row);
    }

    public static string NormalizeDisplayName(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? DefaultDisplayName : trimmed;
    }
}
