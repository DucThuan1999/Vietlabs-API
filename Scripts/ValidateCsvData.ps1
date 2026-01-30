# Script PowerShell để validate CSV files
# Chạy: .\Scripts\ValidateCsvData.ps1

$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "=== KIỂM TRA TÍNH HỢP LỆ CỦA FILE CSV ===" -ForegroundColor Cyan
Write-Host ""

$csvFolder = Join-Path $PSScriptRoot "..\csv"
if (-not (Test-Path $csvFolder)) {
    Write-Host "❌ Không tìm thấy thư mục csv tại: $csvFolder" -ForegroundColor Red
    exit 1
}

Write-Host "📁 Đang đọc từ thư mục: $csvFolder" -ForegroundColor Green
Write-Host ""

$errors = @()
$warnings = @()
$stats = @{}

# Function to parse CSV line (handle quoted fields)
function Parse-CsvLine {
    param([string]$line)
    $result = @()
    $current = ""
    $inQuotes = $false
    
    for ($i = 0; $i -lt $line.Length; $i++) {
        $c = $line[$i]
        if ($c -eq '"') {
            $inQuotes = -not $inQuotes
        }
        elseif ($c -eq ',' -and -not $inQuotes) {
            $result += $current
            $current = ""
        }
        else {
            $current += $c
        }
    }
    $result += $current
    return $result
}

# Read Countries
Write-Host "Đang đọc country.csv..." -ForegroundColor Yellow
$countries = @{}
$countryFile = Join-Path $csvFolder "country.csv"
if (Test-Path $countryFile) {
    $lines = Get-Content $countryFile -Encoding UTF8
    $stats["Countries"] = $lines.Count - 1  # Exclude header
    
    for ($i = 1; $i -lt $lines.Count; $i++) {
        $line = $lines[$i].Trim()
        if ([string]::IsNullOrEmpty($line)) { continue }
        
        $parts = Parse-CsvLine $line
        if ($parts.Count -lt 7) {
            $errors += "country.csv dòng $($i+1): Không đủ cột"
            continue
        }
        
        $nameVi = $parts[2].Trim()
        if ([string]::IsNullOrEmpty($nameVi)) {
            $errors += "country.csv dòng $($i+1): Tên đầy đủ (VI) trống"
        }
        
        if ($countries.ContainsKey($nameVi)) {
            $warnings += "country.csv dòng $($i+1): Quốc gia '$nameVi' trùng lặp"
        }
        $countries[$nameVi] = $parts
    }
} else {
    $errors += "Không tìm thấy file country.csv"
}

# Read Provinces
Write-Host "Đang đọc provinces.csv..." -ForegroundColor Yellow
$provinces = @{}
$provinceFile = Join-Path $csvFolder "provinces.csv"
if (Test-Path $provinceFile) {
    $lines = Get-Content $provinceFile -Encoding UTF8
    $stats["Provinces"] = $lines.Count - 1
    
    for ($i = 1; $i -lt $lines.Count; $i++) {
        $line = $lines[$i].Trim()
        if ([string]::IsNullOrEmpty($line)) { continue }
        
        $parts = Parse-CsvLine $line
        if ($parts.Count -lt 6) {
            $errors += "provinces.csv dòng $($i+1): Không đủ cột"
            continue
        }
        
        $name = $parts[1].Trim()
        $countryName = $parts[4].Trim()
        
        if ([string]::IsNullOrEmpty($name)) {
            $errors += "provinces.csv dòng $($i+1): Tên Tỉnh/Thành phố trống"
        }
        if ([string]::IsNullOrEmpty($countryName)) {
            $errors += "provinces.csv dòng $($i+1): Quốc Gia trống"
        }
        
        # Check foreign key
        if (-not [string]::IsNullOrEmpty($countryName) -and -not $countries.ContainsKey($countryName)) {
            $errors += "provinces.csv dòng $($i+1): Tỉnh '$name' tham chiếu Quốc gia không tồn tại '$countryName'"
        }
        
        if ($provinces.ContainsKey($name)) {
            $warnings += "provinces.csv dòng $($i+1): Tỉnh/Thành phố '$name' trùng lặp"
        }
        $provinces[$name] = $parts
    }
} else {
    $errors += "Không tìm thấy file provinces.csv"
}

# Read Wards
Write-Host "Đang đọc ward.csv..." -ForegroundColor Yellow
$wardFile = Join-Path $csvFolder "ward.csv"
if (Test-Path $wardFile) {
    $lines = Get-Content $wardFile -Encoding UTF8
    $stats["Wards"] = $lines.Count - 1
    
    for ($i = 1; $i -lt $lines.Count; $i++) {
        $line = $lines[$i].Trim()
        if ([string]::IsNullOrEmpty($line)) { continue }
        
        $parts = Parse-CsvLine $line
        if ($parts.Count -lt 7) {
            $errors += "ward.csv dòng $($i+1): Không đủ cột"
            continue
        }
        
        $name = $parts[2].Trim()
        $provinceName = $parts[4].Trim()
        $countryName = $parts[5].Trim()
        
        if ([string]::IsNullOrEmpty($name)) {
            $errors += "ward.csv dòng $($i+1): Tên Xã/Phường trống"
        }
        if ([string]::IsNullOrEmpty($provinceName)) {
            $errors += "ward.csv dòng $($i+1): Tỉnh/Thành Phố trống"
        }
        if ([string]::IsNullOrEmpty($countryName)) {
            $errors += "ward.csv dòng $($i+1): Quốc Gia trống"
        }
        
        # Check foreign keys
        if (-not [string]::IsNullOrEmpty($provinceName) -and -not $provinces.ContainsKey($provinceName)) {
            $errors += "ward.csv dòng $($i+1): Phường/Xã '$name' tham chiếu Tỉnh/Thành phố không tồn tại '$provinceName'"
        }
        if (-not [string]::IsNullOrEmpty($countryName) -and -not $countries.ContainsKey($countryName)) {
            $errors += "ward.csv dòng $($i+1): Phường/Xã '$name' tham chiếu Quốc gia không tồn tại '$countryName'"
        }
        
        # Check if province's country matches ward's country
        if ($provinces.ContainsKey($provinceName) -and -not [string]::IsNullOrEmpty($countryName)) {
            $provinceCountry = $provinces[$provinceName][4].Trim()
            if ($provinceCountry -ne $countryName) {
                $errors += "ward.csv dòng $($i+1): Phường/Xã '$name' có Quốc gia '$countryName' không khớp với Tỉnh '$provinceName' (Quốc gia: '$provinceCountry')"
            }
        }
    }
} else {
    $errors += "Không tìm thấy file ward.csv"
}

# Display results
Write-Host ""
Write-Host "=== THỐNG KÊ ===" -ForegroundColor Cyan
foreach ($key in $stats.Keys) {
    Write-Host "  $key`: $($stats[$key]) bản ghi" -ForegroundColor White
}

Write-Host ""
Write-Host "=== CẢNH BÁO ===" -ForegroundColor Yellow
if ($warnings.Count -eq 0) {
    Write-Host "  ✓ Không có cảnh báo" -ForegroundColor Green
} else {
    foreach ($warning in $warnings) {
        Write-Host "  ⚠ $warning" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== LỖI ===" -ForegroundColor Red
if ($errors.Count -eq 0) {
    Write-Host "  ✓ Không có lỗi" -ForegroundColor Green
} else {
    Write-Host "  ❌ Tổng cộng $($errors.Count) lỗi:" -ForegroundColor Red
    Write-Host ""
    $displayErrors = $errors | Select-Object -First 50
    foreach ($error in $displayErrors) {
        Write-Host "  • $error" -ForegroundColor Red
    }
    if ($errors.Count -gt 50) {
        Write-Host "  ... và $($errors.Count - 50) lỗi khác" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== KẾT LUẬN ===" -ForegroundColor Cyan
if ($errors.Count -eq 0) {
    Write-Host "✅ TẤT CẢ FILE CSV HỢP LỆ - CÓ THỂ INSERT VÀO SQL" -ForegroundColor Green
} else {
    Write-Host "❌ FILE CSV KHÔNG HỢP LỆ - CẦN SỬA LỖI TRƯỚC KHI INSERT" -ForegroundColor Red
}

Write-Host ""
Write-Host "Nhấn phím bất kỳ để thoát..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

