using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace VietLab.Scripts;

public class CsvValidator
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public Dictionary<string, int> Statistics { get; set; } = new Dictionary<string, int>();
    }

    public static ValidationResult ValidateCsvFiles(string csvFolderPath)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            // Đọc và validate Country
            var countries = ReadCountries(Path.Combine(csvFolderPath, "country.csv"), result);
            result.Statistics["Countries"] = countries.Count;

            // Đọc và validate Provinces
            var provinces = ReadProvinces(Path.Combine(csvFolderPath, "provinces.csv"), result);
            result.Statistics["Provinces"] = provinces.Count;

            // Đọc và validate Wards
            var wards = ReadWards(Path.Combine(csvFolderPath, "ward.csv"), result);
            result.Statistics["Wards"] = wards.Count;

            // Validate foreign key relationships
            ValidateRelationships(countries, provinces, wards, result);

            result.IsValid = result.Errors.Count == 0;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Lỗi khi đọc file: {ex.Message}");
        }

        return result;
    }

    private static Dictionary<string, CountryData> ReadCountries(string filePath, ValidationResult result)
    {
        var countries = new Dictionary<string, CountryData>(StringComparer.OrdinalIgnoreCase);
        
        if (!File.Exists(filePath))
        {
            result.Errors.Add($"File không tồn tại: {filePath}");
            return countries;
        }

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length < 2)
        {
            result.Errors.Add("File country.csv không có dữ liệu");
            return countries;
        }

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = ParseCsvLine(line);
            if (parts.Count < 7)
            {
                result.Errors.Add($"Dòng {i + 1} trong country.csv không đủ cột: {line}");
                continue;
            }

            var country = new CountryData
            {
                SequenceNumber = TryParseInt(parts[0]),
                NameEn = parts[1]?.Trim() ?? "",
                FullNameVi = parts[2]?.Trim() ?? "",
                FullNameEn = parts[3]?.Trim() ?? "",
                Alpha2 = parts[4]?.Trim(),
                Alpha3 = parts[5]?.Trim(),
                Status = parts[6]?.Trim() ?? "Active",
                Notes = parts.Count > 7 ? parts[7]?.Trim() : null
            };

            // Validate required fields
            if (string.IsNullOrEmpty(country.NameEn))
            {
                result.Errors.Add($"Dòng {i + 1}: Tên nước (EN) không được để trống");
            }
            if (string.IsNullOrEmpty(country.FullNameVi))
            {
                result.Errors.Add($"Dòng {i + 1}: Tên đầy đủ (VI) không được để trống");
            }
            if (string.IsNullOrEmpty(country.FullNameEn))
            {
                result.Errors.Add($"Dòng {i + 1}: Tên đầy đủ (EN) không được để trống");
            }

            // Check duplicate Alpha-2
            if (!string.IsNullOrEmpty(country.Alpha2))
            {
                var existing = countries.Values.FirstOrDefault(c => c.Alpha2?.Equals(country.Alpha2, StringComparison.OrdinalIgnoreCase) == true);
                if (existing != null)
                {
                    result.Errors.Add($"Dòng {i + 1}: Alpha-2 '{country.Alpha2}' đã tồn tại ở dòng {existing.SequenceNumber}");
                }
            }

            // Check duplicate Alpha-3
            if (!string.IsNullOrEmpty(country.Alpha3))
            {
                var existing = countries.Values.FirstOrDefault(c => c.Alpha3?.Equals(country.Alpha3, StringComparison.OrdinalIgnoreCase) == true);
                if (existing != null)
                {
                    result.Errors.Add($"Dòng {i + 1}: Alpha-3 '{country.Alpha3}' đã tồn tại ở dòng {existing.SequenceNumber}");
                }
            }

            // Use FullNameVi as key for foreign key lookup
            var key = country.FullNameVi;
            if (countries.ContainsKey(key))
            {
                result.Warnings.Add($"Dòng {i + 1}: Quốc gia '{key}' đã tồn tại, sẽ bị ghi đè");
            }
            countries[key] = country;
        }

        return countries;
    }

    private static Dictionary<string, ProvinceData> ReadProvinces(string filePath, ValidationResult result)
    {
        var provinces = new Dictionary<string, ProvinceData>(StringComparer.OrdinalIgnoreCase);
        
        if (!File.Exists(filePath))
        {
            result.Errors.Add($"File không tồn tại: {filePath}");
            return provinces;
        }

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length < 2)
        {
            result.Errors.Add("File provinces.csv không có dữ liệu");
            return provinces;
        }

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = ParseCsvLine(line);
            if (parts.Count < 6)
            {
                result.Errors.Add($"Dòng {i + 1} trong provinces.csv không đủ cột: {line}");
                continue;
            }

            var province = new ProvinceData
            {
                SequenceNumber = TryParseInt(parts[0]),
                Name = parts[1]?.Trim() ?? "",
                Type = parts[2]?.Trim(),
                FullName = parts[3]?.Trim(),
                CountryName = parts[4]?.Trim() ?? "",
                Status = parts[5]?.Trim() ?? "Active",
                Notes = parts.Count > 6 ? parts[6]?.Trim() : null
            };

            // Validate required fields
            if (string.IsNullOrEmpty(province.Name))
            {
                result.Errors.Add($"Dòng {i + 1}: Tên Tỉnh/Thành phố không được để trống");
            }
            if (string.IsNullOrEmpty(province.CountryName))
            {
                result.Errors.Add($"Dòng {i + 1}: Quốc Gia không được để trống");
            }

            // Use Name as key
            var key = province.Name;
            if (provinces.ContainsKey(key))
            {
                result.Warnings.Add($"Dòng {i + 1}: Tỉnh/Thành phố '{key}' đã tồn tại, sẽ bị ghi đè");
            }
            provinces[key] = province;
        }

        return provinces;
    }

    private static List<WardData> ReadWards(string filePath, ValidationResult result)
    {
        var wards = new List<WardData>();
        
        if (!File.Exists(filePath))
        {
            result.Errors.Add($"File không tồn tại: {filePath}");
            return wards;
        }

        var lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length < 2)
        {
            result.Errors.Add("File ward.csv không có dữ liệu");
            return wards;
        }

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = ParseCsvLine(line);
            if (parts.Count < 7)
            {
                result.Errors.Add($"Dòng {i + 1} trong ward.csv không đủ cột: {line}");
                continue;
            }

            var ward = new WardData
            {
                SequenceNumber = TryParseInt(parts[0]),
                Code = parts[1]?.Trim(),
                Name = parts[2]?.Trim() ?? "",
                Type = parts[3]?.Trim(),
                ProvinceName = parts[4]?.Trim() ?? "",
                CountryName = parts[5]?.Trim() ?? "",
                Status = parts[6]?.Trim() ?? "Active",
                Notes = parts.Count > 7 ? parts[7]?.Trim() : null
            };

            // Validate required fields
            if (string.IsNullOrEmpty(ward.Name))
            {
                result.Errors.Add($"Dòng {i + 1}: Tên Xã/Phường không được để trống");
            }
            if (string.IsNullOrEmpty(ward.ProvinceName))
            {
                result.Errors.Add($"Dòng {i + 1}: Tỉnh/Thành Phố không được để trống");
            }
            if (string.IsNullOrEmpty(ward.CountryName))
            {
                result.Errors.Add($"Dòng {i + 1}: Quốc Gia không được để trống");
            }

            wards.Add(ward);
        }

        return wards;
    }

    private static void ValidateRelationships(
        Dictionary<string, CountryData> countries,
        Dictionary<string, ProvinceData> provinces,
        List<WardData> wards,
        ValidationResult result)
    {
        // Validate Provinces -> Countries
        var missingCountries = new HashSet<string>();
        foreach (var province in provinces.Values)
        {
            if (!countries.ContainsKey(province.CountryName))
            {
                missingCountries.Add(province.CountryName);
                result.Errors.Add($"Tỉnh/Thành phố '{province.Name}' tham chiếu đến Quốc gia không tồn tại: '{province.CountryName}'");
            }
        }

        if (missingCountries.Count > 0)
        {
            result.Warnings.Add($"Tổng cộng {missingCountries.Count} Quốc gia không tồn tại được tham chiếu từ Provinces");
        }

        // Validate Wards -> Provinces
        var missingProvinces = new HashSet<string>();
        var missingCountriesFromWards = new HashSet<string>();
        
        foreach (var ward in wards)
        {
            if (!provinces.ContainsKey(ward.ProvinceName))
            {
                missingProvinces.Add(ward.ProvinceName);
                result.Errors.Add($"Phường/Xã '{ward.Name}' (Mã: {ward.Code}) tham chiếu đến Tỉnh/Thành phố không tồn tại: '{ward.ProvinceName}'");
            }
            else
            {
                // Check if ward's country matches province's country
                var province = provinces[ward.ProvinceName];
                if (!province.CountryName.Equals(ward.CountryName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Errors.Add($"Phường/Xã '{ward.Name}' có Quốc gia '{ward.CountryName}' không khớp với Tỉnh/Thành phố '{ward.ProvinceName}' (Quốc gia: '{province.CountryName}')");
                }
            }

            if (!countries.ContainsKey(ward.CountryName))
            {
                missingCountriesFromWards.Add(ward.CountryName);
                result.Errors.Add($"Phường/Xã '{ward.Name}' tham chiếu đến Quốc gia không tồn tại: '{ward.CountryName}'");
            }
        }

        if (missingProvinces.Count > 0)
        {
            result.Warnings.Add($"Tổng cộng {missingProvinces.Count} Tỉnh/Thành phố không tồn tại được tham chiếu từ Wards");
        }
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString());

        return result;
    }

    private static int? TryParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (int.TryParse(value, out int result))
            return result;
        return null;
    }

    public class CountryData
    {
        public int? SequenceNumber { get; set; }
        public string NameEn { get; set; } = "";
        public string FullNameVi { get; set; } = "";
        public string FullNameEn { get; set; } = "";
        public string? Alpha2 { get; set; }
        public string? Alpha3 { get; set; }
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
    }

    public class ProvinceData
    {
        public int? SequenceNumber { get; set; }
        public string Name { get; set; } = "";
        public string? Type { get; set; }
        public string? FullName { get; set; }
        public string CountryName { get; set; } = "";
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
    }

    public class WardData
    {
        public int? SequenceNumber { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = "";
        public string? Type { get; set; }
        public string ProvinceName { get; set; } = "";
        public string CountryName { get; set; } = "";
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
    }
}

