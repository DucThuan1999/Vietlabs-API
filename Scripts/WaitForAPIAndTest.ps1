# Script cho API restart va test lai
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\WaitForAPIAndTest.ps1

$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "=== Cho API restart va test lai ===" -ForegroundColor Green
Write-Host ""

# Cho API khoi dong (toi da 30 giay)
Write-Host "Dang cho API khoi dong..." -ForegroundColor Yellow
$maxWait = 30
$waited = 0
$apiReady = $false

while ($waited -lt $maxWait -and -not $apiReady) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/odata" -Method Get -ContentType "application/json" -TimeoutSec 2 -ErrorAction Stop
        $apiReady = $true
        Write-Host "  API da san sang!" -ForegroundColor Green
    } catch {
        Start-Sleep -Seconds 2
        $waited += 2
        Write-Host "  Dang cho... ($waited/$maxWait giay)" -ForegroundColor Gray
    }
}

if (-not $apiReady) {
    Write-Host "  WARNING: API chua khoi dong sau $maxWait giay" -ForegroundColor Yellow
    Write-Host "  Vui long start API thu cong va chay lai script test" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "=== Test POST QuotationItem voi cac fields moi ===" -ForegroundColor Green
Write-Host ""

$quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
$analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
$endpoint = "$baseUrl/odata/QuotationItems"

# Test voi body day du
$body = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    isStandalone = $true
    defaultPrice = 200000
    itemCode = "CT-0005"
    itemNameVi = "Sulfadimethoxine"
    itemNameEn = "Sulfadimethoxine"
    sampleMatrixName = "Thuc pham"
    publishedGroupCode = "VLAB-CH-TP-659"
    unit = "ug/kg"
    lod = "0.167"
    loq = "0.5"
    tat = "7"
    displayOrder = 1
    discountPercent = 0
    discountAmount = 0
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Cyan
Write-Host $body -ForegroundColor Gray
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body -ContentType "application/json" -ErrorAction Stop
    
    Write-Host "SUCCESS - QuotationItem da duoc tao!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Response:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host ""
    Write-Host "=== Kiem tra cac fields moi ===" -ForegroundColor Yellow
    $checks = @(
        @{Field="isStandalone"; Value=$response.isStandalone; Expected=$true},
        @{Field="defaultPrice"; Value=$response.defaultPrice; Expected=200000},
        @{Field="sampleMatrixName"; Value=$response.sampleMatrixName; Expected="Thuc pham"},
        @{Field="publishedGroupCode"; Value=$response.publishedGroupCode; Expected="VLAB-CH-TP-659"},
        @{Field="unit"; Value=$response.unit; Expected="ug/kg"},
        @{Field="lod"; Value=$response.lod; Expected="0.167"},
        @{Field="loq"; Value=$response.loq; Expected="0.5"},
        @{Field="tat"; Value=$response.tat; Expected="7"}
    )
    
    $allPassed = $true
    foreach ($check in $checks) {
        if ($check.Value -eq $check.Expected) {
            Write-Host "  OK $($check.Field): $($check.Value)" -ForegroundColor Green
        } else {
            Write-Host "  WARNING $($check.Field): $($check.Value) (Expected: $($check.Expected))" -ForegroundColor Yellow
            $allPassed = $false
        }
    }
    
    Write-Host ""
    if ($allPassed) {
        Write-Host "=== Test Complete ===" -ForegroundColor Green
        Write-Host "TAT CA CAC FIELDS MOI DA HOAT DONG!" -ForegroundColor Green
        Write-Host "QuotationItemId: $($response.quotationItemId)" -ForegroundColor Green
    } else {
        Write-Host "=== Test Complete (co canh bao) ===" -ForegroundColor Yellow
        Write-Host "Co mot so fields chua duoc luu dung" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "Response: $errorBody" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "Van de co the la:" -ForegroundColor Yellow
    Write-Host "  1. API chua nhan dien cac columns moi" -ForegroundColor Yellow
    Write-Host "  2. Can rebuild project" -ForegroundColor Yellow
    Write-Host "  3. Can check Entity Framework configuration" -ForegroundColor Yellow
}

