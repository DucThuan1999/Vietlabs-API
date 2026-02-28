using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;

namespace VietLab.Helpers;

public static class ControllerExtensions
{
    /// <summary>
    /// Xử lý DbUpdateException và trả về BadRequest với thông báo lỗi chi tiết
    /// </summary>
    public static IActionResult HandleDatabaseError(
        this ControllerBase controller,
        DbUpdateException ex,
        ILogger logger,
        string operation = "thao tác")
    {
        logger.LogError(ex, "Error during {Operation}", operation);
        var errorResponse = DatabaseErrorHandler.GetDetailedErrorResponse(ex, logger);
        return controller.BadRequest(errorResponse);
    }

    /// <summary>
    /// Thực thi SaveChangesAsync với xử lý lỗi tự động
    /// </summary>
    public static async Task<IActionResult?> SaveChangesWithErrorHandlingAsync(
        this ControllerBase controller,
        ApplicationDbContext context,
        ILogger logger,
        Func<Task<IActionResult>> onSuccess,
        string operation = "thao tác")
    {
        try
        {
            await context.SaveChangesAsync();
            return await onSuccess();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrency error during {Operation}", operation);
            return controller.Conflict(new
            {
                error = "Lỗi đồng thời",
                message = "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng làm mới và thử lại."
            });
        }
        catch (DbUpdateException ex)
        {
            return controller.HandleDatabaseError(ex, logger, operation);
        }
    }
}

