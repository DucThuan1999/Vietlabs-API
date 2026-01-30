# Script test POST sau khi update database
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestPostAfterDBUpdate.ps1

$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
$analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
$endpoint = "$baseUrl/odata/QuotationItems"

Write-Host "=== Test POST sau khi update database ===" -ForegroundColor Green
Write-Host "Luu y: Neu van loi, can RESTART API de Entity Framework nhan dien columns moi" -ForegroundColor Yellow
Write-Host ""

# Test 1: Body co ban (da thanh cong)
Write-Host "TEST 1: Body co ban (da thanh cong truoc do)" -ForegroundColor Yellow
$body1 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
} | ConvertTo-Json

try {
    $response1 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body1 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - Body co ban OK" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response1.quotationItemId)" -ForegroundColor Green
} catch {
    Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Them isStandalone
Write-Host "TEST 2: Body co ban + isStandalone" -ForegroundColor Yellow
$body2 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    isStandalone = $true
} | ConvertTo-Json

try {
    $response2 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body2 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - isStandalone OK!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response2.quotationItemId)" -ForegroundColor Green
    Write-Host "  isStandalone: $($response2.isStandalone)" -ForegroundColor Cyan
} catch {
    Write-Host "  ERROR - isStandalone: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "  Response: $($reader.ReadToEnd())" -ForegroundColor Red
    }
}
Write-Host ""

# Test 3: Them defaultPrice
Write-Host "TEST 3: Body co ban + defaultPrice" -ForegroundColor Yellow
$body3 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    defaultPrice = 200000
} | ConvertTo-Json

try {
    $response3 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body3 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - defaultPrice OK!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response3.quotationItemId)" -ForegroundColor Green
    Write-Host "  defaultPrice: $($response3.defaultPrice)" -ForegroundColor Cyan
} catch {
    Write-Host "  ERROR - defaultPrice: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Body co ban + 1 snapshot field (sampleMatrixName)
Write-Host "TEST 4: Body co ban + sampleMatrixName" -ForegroundColor Yellow
$body4 = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
    sampleMatrixName = "Thuc pham"
} | ConvertTo-Json

try {
    $response4 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body4 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - sampleMatrixName OK!" -ForegroundColor Green
    Write-Host "  QuotationItemId: $($response4.quotationItemId)" -ForegroundColor Green
    Write-Host "  sampleMatrixName: $($response4.sampleMatrixName)" -ForegroundColor Cyan
} catch {
    Write-Host "  ERROR - sampleMatrixName: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "  Response: $($reader.ReadToEnd())" -ForegroundColor Red
    }
}
Write-Host ""

# Test 5: Body day du (tat ca fields)
Write-Host "TEST 5: Body day du (tat ca fields)" -ForegroundColor Yellow
$body5 = @{
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

Write-Host "  Request Body:" -ForegroundColor Cyan
Write-Host $body5 -ForegroundColor Gray
Write-Host ""

try {
    $response5 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body5 -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - Body day du OK!" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Response:" -ForegroundColor Green
    $response5 | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host ""
    Write-Host "  Kiem tra cac fields:" -ForegroundColor Yellow
    Write-Host "    isStandalone: $($response5.isStandalone)" -ForegroundColor $(if ($response5.isStandalone) { "Green" } else { "Yellow" })
    Write-Host "    defaultPrice: $($response5.defaultPrice)" -ForegroundColor $(if ($response5.defaultPrice) { "Green" } else { "Yellow" })
    Write-Host "    sampleMatrixName: $($response5.sampleMatrixName)" -ForegroundColor $(if ($response5.sampleMatrixName) { "Green" } else { "Yellow" })
    Write-Host "    publishedGroupCode: $($response5.publishedGroupCode)" -ForegroundColor $(if ($response5.publishedGroupCode) { "Green" } else { "Yellow" })
    Write-Host "    unit: $($response5.unit)" -ForegroundColor $(if ($response5.unit) { "Green" } else { "Yellow" })
    Write-Host "    lod: $($response5.lod)" -ForegroundColor $(if ($response5.lod) { "Green" } else { "Yellow" })
    Write-Host "    loq: $($response5.loq)" -ForegroundColor $(if ($response5.loq) { "Green" } else { "Yellow" })
    Write-Host "    tat: $($response5.tat)" -ForegroundColor $(if ($response5.tat) { "Green" } else { "Yellow" })
    
} catch {
    Write-Host "  ERROR - Body day du: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Response: $errorBody" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "  KHUYEN NGHI: Restart API de Entity Framework nhan dien cac columns moi!" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Green

