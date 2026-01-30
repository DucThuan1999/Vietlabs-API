# Script test POST QuotationItem - Test rieng tung field gay loi
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotationItemProblemFields.ps1

$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

$quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
$analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
$endpoint = "$baseUrl/odata/QuotationItems"

Write-Host "=== Test rieng tung field gay loi ===" -ForegroundColor Green
Write-Host ""

# Body co ban (da thanh cong)
$baseBody = @{
    quotationId = $quotationId
    itemType = "AnalysisItem"
    quantity = 1
    unitPrice = 200000
    subTotal = 200000
    analysisItemId = $analysisItemId
}

# Test 1: isStandalone
Write-Host "TEST 1: Them isStandalone" -ForegroundColor Yellow
$body1 = $baseBody.Clone()
$body1.isStandalone = $true
try {
    $response1 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body1 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - isStandalone OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - isStandalone: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "  Response: $($reader.ReadToEnd())" -ForegroundColor Red
    }
}
Write-Host ""

# Test 2: defaultPrice
Write-Host "TEST 2: Them defaultPrice" -ForegroundColor Yellow
$body2 = $baseBody.Clone()
$body2.defaultPrice = 200000
try {
    $response2 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body2 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - defaultPrice OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - defaultPrice: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: discountPercent
Write-Host "TEST 3: Them discountPercent" -ForegroundColor Yellow
$body3 = $baseBody.Clone()
$body3.discountPercent = 0
try {
    $response3 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body3 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - discountPercent OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - discountPercent: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: discountAmount
Write-Host "TEST 4: Them discountAmount" -ForegroundColor Yellow
$body4 = $baseBody.Clone()
$body4.discountAmount = 0
try {
    $response4 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body4 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - discountAmount OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - discountAmount: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: sampleMatrixName
Write-Host "TEST 5: Them sampleMatrixName" -ForegroundColor Yellow
$body5 = $baseBody.Clone()
$body5.sampleMatrixName = "Thuc pham"
try {
    $response5 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body5 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - sampleMatrixName OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - sampleMatrixName: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 6: publishedGroupCode
Write-Host "TEST 6: Them publishedGroupCode" -ForegroundColor Yellow
$body6 = $baseBody.Clone()
$body6.publishedGroupCode = "VLAB-CH-TP-659"
try {
    $response6 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body6 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - publishedGroupCode OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - publishedGroupCode: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 7: unit
Write-Host "TEST 7: Them unit" -ForegroundColor Yellow
$body7 = $baseBody.Clone()
$body7.unit = "ug/kg"
try {
    $response7 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body7 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - unit OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - unit: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 8: lod
Write-Host "TEST 8: Them lod" -ForegroundColor Yellow
$body8 = $baseBody.Clone()
$body8.lod = "0.167"
try {
    $response8 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body8 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - lod OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - lod: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 9: loq
Write-Host "TEST 9: Them loq" -ForegroundColor Yellow
$body9 = $baseBody.Clone()
$body9.loq = "0.5"
try {
    $response9 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body9 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - loq OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - loq: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 10: tat
Write-Host "TEST 10: Them tat" -ForegroundColor Yellow
$body10 = $baseBody.Clone()
$body10.tat = "7"
try {
    $response10 = Invoke-RestMethod -Uri $endpoint -Method Post -Body ($body10 | ConvertTo-Json) -ContentType "application/json" -ErrorAction Stop
    Write-Host "  SUCCESS - tat OK" -ForegroundColor Green
} catch {
    Write-Host "  ERROR - tat: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== Test Complete ===" -ForegroundColor Green

