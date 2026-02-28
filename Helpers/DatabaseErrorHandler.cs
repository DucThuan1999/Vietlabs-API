using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace VietLab.Helpers;

public static class DatabaseErrorHandler
{
    /// <summary>
    /// Xử lý lỗi DbUpdateException và trả về thông báo lỗi chi tiết bằng tiếng Việt
    /// </summary>
    public static object GetDetailedErrorResponse(DbUpdateException ex, ILogger? logger = null)
    {
        logger?.LogError(ex, "Database error occurred");

        // Kiểm tra nếu là SqlException để lấy thông tin chi tiết
        if (ex.InnerException is SqlException sqlEx)
        {
            var errorMessage = sqlEx.Number switch
            {
                2628 => GetTruncationErrorMessage(sqlEx),
                547 => $"Vi phạm ràng buộc foreign key hoặc check constraint. {GetConstraintDetails(sqlEx.Message)}",
                515 => $"Giá trị NULL không được phép trong cột bắt buộc. {GetColumnNameFromError(sqlEx.Message)}",
                2601 => $"Vi phạm unique constraint - dữ liệu đã tồn tại. {GetConstraintDetails(sqlEx.Message)}",
                8152 => $"Dữ liệu quá dài cho cột. {GetColumnNameFromError(sqlEx.Message)}",
                241 => $"Lỗi chuyển đổi dữ liệu. {sqlEx.Message}",
                245 => $"Lỗi chuyển đổi kiểu dữ liệu. {sqlEx.Message}",
                _ => sqlEx.Message
            };

            return new
            {
                error = "Lỗi cơ sở dữ liệu",
                message = errorMessage,
                sqlError = new
                {
                    number = sqlEx.Number,
                    state = sqlEx.State,
                    @class = sqlEx.Class,
                    server = sqlEx.Server,
                    procedure = sqlEx.Procedure,
                    lineNumber = sqlEx.LineNumber,
                    originalMessage = sqlEx.Message
                }
            };
        }

        // Nếu không phải SqlException, trả về message thông thường
        return new
        {
            error = "Lỗi cơ sở dữ liệu",
            message = ex.InnerException?.Message ?? ex.Message
        };
    }

    /// <summary>
    /// Xử lý lỗi truncation (2628) với thông báo chi tiết
    /// </summary>
    private static string GetTruncationErrorMessage(SqlException sqlEx)
    {
        var columnName = GetColumnNameFromError(sqlEx.Message);
        var truncatedValue = GetTruncatedValue(sqlEx.Message);
        var tableName = GetTableNameFromError(sqlEx.Message);

        var message = $"Dữ liệu bị cắt ngắn trong bảng '{tableName}', cột '{columnName}'.";
        
        if (!string.IsNullOrEmpty(truncatedValue))
        {
            message += $" Giá trị bị cắt: '{truncatedValue}'. ";
        }
        
        message += "Vui lòng kiểm tra độ dài dữ liệu phù hợp với cấu trúc database.";
        
        return message;
    }

    /// <summary>
    /// Lấy tên cột từ error message
    /// </summary>
    private static string GetColumnNameFromError(string errorMessage)
    {
        // Tìm tên cột từ error message: "column 'column_name'"
        var match = Regex.Match(
            errorMessage,
            @"column\s+'([^']+)'",
            RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : "không xác định";
    }

    /// <summary>
    /// Lấy giá trị bị cắt từ error message
    /// </summary>
    private static string GetTruncatedValue(string errorMessage)
    {
        // Tìm giá trị bị cắt từ error message: "Truncated value: 'value'"
        var match = Regex.Match(
            errorMessage,
            @"Truncated value:\s*'([^']*)'",
            RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    /// <summary>
    /// Lấy tên bảng từ error message
    /// </summary>
    private static string GetTableNameFromError(string errorMessage)
    {
        // Tìm tên bảng từ error message: "table 'database.schema.table'"
        var match = Regex.Match(
            errorMessage,
            @"table\s+'([^']+)'",
            RegexOptions.IgnoreCase);
        
        if (match.Success)
        {
            var fullTableName = match.Groups[1].Value;
            // Lấy phần cuối cùng sau dấu chấm (tên bảng)
            var parts = fullTableName.Split('.');
            return parts.Length > 0 ? parts[^1] : fullTableName;
        }
        
        return "không xác định";
    }

    /// <summary>
    /// Lấy chi tiết constraint từ error message
    /// </summary>
    private static string GetConstraintDetails(string errorMessage)
    {
        // Tìm tên constraint hoặc object từ error message
        var match = Regex.Match(
            errorMessage,
            @"(?:constraint|object)\s+'([^']+)'",
            RegexOptions.IgnoreCase);
        return match.Success ? $"Constraint/Object: {match.Groups[1].Value}" : string.Empty;
    }
}

