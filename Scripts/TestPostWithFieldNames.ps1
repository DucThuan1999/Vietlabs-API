# Script test POST voi cac field names khac nhau
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestPostWithFieldNames.ps1

$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
$analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
$endpoint = "$baseUrl/odata/QuotationItems"

Write-Host "=== Test POST voi cac field names khac nhau ===" -ForegroundColor Green
Write-Host ""

# Test 1: camelCase (như da test truoc do)
Write-Host "TEST 1: POST voi camelCase" -ForegroundColor Yellow
$body1 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    itemCode = "CT-0005"
    itemNameVi = "Test Item"
    isStandalone = $true
    defaultPrice = 200000
    sampleMatrixName = "Thuc pham"
    publishedGroupCode = "VLAB-CH-TP-659"
    unit = "ug/kg"
    lod = "0.167"
    loq = "0.5"
    tat = "7"
} | ConvertTo-Json

Write-Host "  Body (camelCase):" -ForegroundColor Cyan
Write-Host $body1 -ForegroundColor Gray
Write-Host ""

try {
    $response1 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body1 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - camelCase OK!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response1.quotationItemId)" -ForegroundColor Green
    Write-Host "  Response fields:" -ForegroundColor Cyan
    Write-Host "    isStandalone: $($response1.isStandalone)" -ForegroundColor Gray
    Write-Host "    defaultPrice: $($response1.defaultPrice)" -ForegroundColor Gray
    Write-Host "    sampleMatrixName: $($response1.sampleMatrixName)" -ForegroundColor Gray
} catch {
    Write-Host "  ERROR - camelCase: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Response: $errorBody" -ForegroundColor Red
    }
}
Write-Host ""

# Test 2: PascalCase (như OData tra ve)
Write-Host "TEST 2: POST voi PascalCase" -ForegroundColor Yellow
$body2 = @{
    QuotationId = $quotationId
    ItemType = "AnalysisItem"
    Quantity = 1
    UnitPrice = 200000
    SubTotal = 200000
    AnalysisItemId = $analysisItemId
    ItemCode = "CT-0005"
    ItemNameVi = "Test Item 2"
    IsStandalone = $true
    DefaultPrice = 200000
    SampleMatrixName = "Thuc pham"
    PublishedGroupCode = "VLAB-CH-TP-659"
    Unit = "ug/kg"
    Lod = "0.167"
    Loq = "0.5"
    Tat = "7"
} | ConvertTo-Json

Write-Host "  Body (PascalCase):" -ForegroundColor Cyan
Write-Host $body2 -ForegroundColor Gray
Write-Host ""

try {
    $response2 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body2 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - PascalCase OK!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response2.quotationItemId)" -ForegroundColor Green
    Write-Host "  Response fields:" -ForegroundColor Cyan
    Write-Host "    IsStandalone: $($response2.IsStandalone)" -ForegroundColor Gray
    Write-Host "    DefaultPrice: $($response2.DefaultPrice)" -ForegroundColor Gray
    Write-Host "    SampleMatrixName: $($response2.SampleMatrixName)" -ForegroundColor Gray
} catch {
    Write-Host "  ERROR - PascalCase: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Response: $errorBody" -ForegroundColor Red
    }
}
Write-Host ""

# Test 3: Chi voi cac fields co ban (khong co snapshot fields)
Write-Host "TEST 3: POST voi cac fields co ban + isStandalone + defaultPrice" -ForegroundColor Yellow
$body3 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    isStandalone = $true
    defaultPrice = 200000
} | ConvertTo-Json

Write-Host "  Body (chi fields co ban + isStandalone + defaultPrice):" -ForegroundColor Cyan
Write-Host $body3 -ForegroundColor Gray
Write-Host ""

try {
    $response3 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body3 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response3.quotationItemId)" -ForegroundColor Green
    Write-Host "  isStandalone: $($response3.isStandalone)" -ForegroundColor Gray
    Write-Host "  defaultPrice: $($response3.defaultPrice)" -ForegroundColor Gray
} catch {
    Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Response: $errorBody" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Green

