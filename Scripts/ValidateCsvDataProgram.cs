using System;
using System.IO;
using System.Linq;
using VietLab.Scripts;

namespace VietLab.Scripts;

class ValidateCsvProgram
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== KIỂM TRA TÍNH HỢP LỆ CỦA FILE CSV ===\n");

        // Tìm thư mục csv (có thể ở root hoặc trong project)
        var csvFolder = Path.Combine(Directory.GetCurrentDirectory(), "csv");
        if (!Directory.Exists(csvFolder))
        {
            csvFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "csv");
        }
        if (!Directory.Exists(csvFolder))
        {
            Console.WriteLine("❌ Không tìm thấy thư mục 'csv'");
            Console.WriteLine($"Đang tìm tại: {csvFolder}");
            return;
        }

        Console.WriteLine($"📁 Đang đọc từ thư mục: {csvFolder}\n");

        var result = CsvValidator.ValidateCsvFiles(csvFolder);

        // Hiển thị kết quả
        Console.WriteLine("=== THỐNG KÊ ===");
        foreach (var stat in result.Statistics)
        {
            Console.WriteLine($"  {stat.Key}: {stat.Value} bản ghi");
        }

        Console.WriteLine("\n=== CẢNH BÁO ===");
        if (result.Warnings.Count == 0)
        {
            Console.WriteLine("  ✓ Không có cảnh báo");
        }
        else
        {
            foreach (var warning in result.Warnings)
            {
                Console.WriteLine($"  ⚠ {warning}");
            }
        }

        Console.WriteLine("\n=== LỖI ===");
        if (result.Errors.Count == 0)
        {
            Console.WriteLine("  ✓ Không có lỗi");
        }
        else
        {
            Console.WriteLine($"  ❌ Tổng cộng {result.Errors.Count} lỗi:\n");
            foreach (var error in result.Errors.Take(50)) // Hiển thị tối đa 50 lỗi đầu tiên
            {
                Console.WriteLine($"  • {error}");
            }
            if (result.Errors.Count > 50)
            {
                Console.WriteLine($"  ... và {result.Errors.Count - 50} lỗi khác");
            }
        }

        Console.WriteLine("\n=== KẾT LUẬN ===");
        if (result.IsValid)
        {
            Console.WriteLine("✅ TẤT CẢ FILE CSV HỢP LỆ - CÓ THỂ INSERT VÀO SQL");
        }
        else
        {
            Console.WriteLine("❌ FILE CSV KHÔNG HỢP LỆ - CẦN SỬA LỖI TRƯỚC KHI INSERT");
        }

        Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }
}

