# Script kiem tra mapping giua fields bi loi va database columns
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\CheckFieldsMapping.ps1

Write-Host "=== Kiem tra mapping giua fields bi loi va database columns ===" -ForegroundColor Green
Write-Host ""

# Fields bi loi (tu test)
$errorFields = @(
    "isStandalone",
    "defaultPrice",
    "sampleMatrixName",
    "publishedGroupCode",
    "unit",
    "lod",
    "loq",
    "tat"
)

# Database columns (tu cau lenh SELECT)
$dbColumns = @(
    "quotation_item_id",
    "quotation_id",
    "item_type",
    "analysis_item_id",
    "analysis_group_id",
    "package_id",
    "item_code",
    "item_name_vi",
    "item_name_en",
    "description",
    "quantity",
    "unit_price",
    "discount_percent",
    "discount_amount",
    "sub_total",
    "display_order",
    "notes",
    "created_at",
    "updated_at",
    "is_standalone",
    "sample_matrix_name",
    "published_group_code",
    "unit",
    "lod",
    "loq",
    "tat",
    "default_price"
)

# Mapping tu Model (PascalCase) -> Database (snake_case)
$expectedMapping = @{
    "IsStandalone" = "is_standalone"
    "DefaultPrice" = "default_price"
    "SampleMatrixName" = "sample_matrix_name"
    "PublishedGroupCode" = "published_group_code"
    "Unit" = "unit"
    "Lod" = "lod"
    "Loq" = "loq"
    "Tat" = "tat"
}

Write-Host "=== Fields bi loi (camelCase trong request) ===" -ForegroundColor Yellow
foreach ($field in $errorFields) {
    Write-Host "  - $field" -ForegroundColor Gray
}
Write-Host ""

Write-Host "=== Database columns (snake_case) ===" -ForegroundColor Yellow
foreach ($col in $dbColumns) {
    Write-Host "  - $col" -ForegroundColor Gray
}
Write-Host ""

Write-Host "=== Kiem tra mapping ===" -ForegroundColor Yellow
Write-Host ""

$allMatched = $true

foreach ($field in $errorFields) {
    # Convert camelCase to PascalCase
    $pascalCase = $field.Substring(0,1).ToUpper() + $field.Substring(1)
    
    # Convert camelCase to snake_case (expected)
    $snakeCase = ""
    for ($i = 0; $i -lt $field.Length; $i++) {
        $char = $field[$i]
        if ([char]::IsUpper($char) -and $i -gt 0) {
            $snakeCase += "_"
        }
        $snakeCase += $char.ToLower()
    }
    
    # Check if exists in expected mapping
    if ($expectedMapping.ContainsKey($pascalCase)) {
        $expectedDbCol = $expectedMapping[$pascalCase]
    } else {
        $expectedDbCol = $snakeCase
    }
    
    # Check if column exists in database
    $exists = $dbColumns -contains $expectedDbCol
    
    if ($exists) {
        Write-Host "  OK $field (PascalCase: $pascalCase) -> $expectedDbCol" -ForegroundColor Green
    } else {
        Write-Host "  ERROR $field (PascalCase: $pascalCase) -> $expectedDbCol (KHONG TIM THAY)" -ForegroundColor Red
        $allMatched = $false
    }
}

Write-Host ""
Write-Host "=== Ket qua ===" -ForegroundColor Yellow

if ($allMatched) {
    Write-Host "  TAT CA CAC FIELDS DEU CO TRONG DATABASE!" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Van de co the la:" -ForegroundColor Yellow
    Write-Host "    1. API chua restart sau khi them columns" -ForegroundColor Yellow
    Write-Host "    2. Entity Framework chua nhan dien cac columns moi" -ForegroundColor Yellow
    Write-Host "    3. OData model binding chua cap nhat" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  GIAI PHAP: Restart API de Entity Framework nhan dien cac columns moi!" -ForegroundColor Cyan
} else {
    Write-Host "  CO MOT SO FIELDS CHUA CO TRONG DATABASE!" -ForegroundColor Red
    Write-Host "  Can them cac columns con thieu vao database." -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Chi tiet mapping (tu EF Configuration) ===" -ForegroundColor Yellow
Write-Host "  IsStandalone -> is_standalone" -ForegroundColor $(if ($dbColumns -contains "is_standalone") { "Green" } else { "Red" })
Write-Host "  DefaultPrice -> default_price" -ForegroundColor $(if ($dbColumns -contains "default_price") { "Green" } else { "Red" })
Write-Host "  SampleMatrixName -> sample_matrix_name" -ForegroundColor $(if ($dbColumns -contains "sample_matrix_name") { "Green" } else { "Red" })
Write-Host "  PublishedGroupCode -> published_group_code" -ForegroundColor $(if ($dbColumns -contains "published_group_code") { "Green" } else { "Red" })
Write-Host "  Unit -> unit" -ForegroundColor $(if ($dbColumns -contains "unit") { "Green" } else { "Red" })
Write-Host "  Lod -> lod" -ForegroundColor $(if ($dbColumns -contains "lod") { "Green" } else { "Red" })
Write-Host "  Loq -> loq" -ForegroundColor $(if ($dbColumns -contains "loq") { "Green" } else { "Red" })
Write-Host "  Tat -> tat" -ForegroundColor $(if ($dbColumns -contains "tat") { "Green" } else { "Red" })

