# Script test POST QuotationItem - Test tung field mot
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotationItemFieldsStepByStep.ps1

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5000"
Write-Host "=== Test POST QuotationItem (Test tung field) ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host ""

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Lay QuotationId va AnalysisItemId
    $quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
    $analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
    
    $endpoint = "$baseUrl/odata/QuotationItems"
    
    # Test 1: Body toi thieu (da thanh cong truoc do)
    Write-Host "=== TEST 1: Body toi thieu ===" -ForegroundColor Yellow
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
        Write-Host "  SUCCESS - Body toi thieu thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response1.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
    
    # Test 2: Them cac fields co ban
    Write-Host "=== TEST 2: Them cac fields co ban ===" -ForegroundColor Yellow
    $body2 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        analysisItemId = $analysisItemId
        itemCode = "CT-0005"
        itemNameVi = "Sulfadimethoxine"
        itemNameEn = "Sulfadimethoxine"
    } | ConvertTo-Json
    
    try {
        $response2 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body2 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Them fields co ban thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response2.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $responseBody = $reader.ReadToEnd()
            Write-Host "  Response: $responseBody" -ForegroundColor Red
        }
    }
    Write-Host ""
    
    # Test 3: Them isStandalone
    Write-Host "=== TEST 3: Them isStandalone ===" -ForegroundColor Yellow
    $body3 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        analysisItemId = $analysisItemId
        isStandalone = $true
    } | ConvertTo-Json
    
    try {
        $response3 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body3 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Them isStandalone thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response3.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
    
    # Test 4: Them defaultPrice, discountPercent, discountAmount
    Write-Host "=== TEST 4: Them defaultPrice, discountPercent, discountAmount ===" -ForegroundColor Yellow
    $body4 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        analysisItemId = $analysisItemId
        defaultPrice = 200000
        discountPercent = 0
        discountAmount = 0
    } | ConvertTo-Json
    
    try {
        $response4 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body4 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Them price fields thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response4.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
    
    # Test 5: Them displayOrder
    Write-Host "=== TEST 5: Them displayOrder ===" -ForegroundColor Yellow
    $body5 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        analysisItemId = $analysisItemId
        displayOrder = 1
    } | ConvertTo-Json
    
    try {
        $response5 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body5 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Them displayOrder thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response5.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
    
    # Test 6: Them snapshot fields (sampleMatrixName, publishedGroupCode, unit, lod, loq, tat)
    Write-Host "=== TEST 6: Them snapshot fields ===" -ForegroundColor Yellow
    $body6 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        analysisItemId = $analysisItemId
        sampleMatrixName = "Thuc pham"
        publishedGroupCode = "VLAB-CH-TP-659"
        unit = "ug/kg"
        lod = "0.167"
        loq = "0.5"
        tat = "7"
    } | ConvertTo-Json
    
    try {
        $response6 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body6 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Them snapshot fields thanh cong!" -ForegroundColor Green
        Write-Host "  QuotationItemId: $($response6.quotationItemId)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $responseBody = $reader.ReadToEnd()
            Write-Host "  Response: $responseBody" -ForegroundColor Red
        }
    }
    Write-Host ""
    
    # Test 7: Body day du (tat ca fields)
    Write-Host "=== TEST 7: Body day du (tat ca fields) ===" -ForegroundColor Yellow
    $body7 = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        quantity = 1
        unitPrice = 200000
        subTotal = 200000
        itemCode = "CT-0005"
        itemNameVi = "Sulfadimethoxine"
        itemNameEn = "Sulfadimethoxine"
        defaultPrice = 200000
        discountPercent = 0
        discountAmount = 0
        displayOrder = 1
        analysisItemId = $analysisItemId
        isStandalone = $true
        sampleMatrixName = "Thuc pham"
        publishedGroupCode = "VLAB-CH-TP-659"
        unit = "ug/kg"
        lod = "0.167"
        loq = "0.5"
        tat = "7"
    } | ConvertTo-Json
    
    Write-Host "  Request Body:" -ForegroundColor Cyan
    Write-Host $body7 -ForegroundColor Gray
    Write-Host ""
    
    try {
        $response7 = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body7 -ContentType "application/json" -ErrorAction Stop
        Write-Host "  SUCCESS - Body day du thanh cong!" -ForegroundColor Green
        Write-Host ""
        Write-Host "  Response:" -ForegroundColor Green
        $response7 | ConvertTo-Json -Depth 10 | Write-Host
        
        # Kiem tra cac fields
        Write-Host ""
        Write-Host "  Kiem tra cac fields:" -ForegroundColor Yellow
        $checks = @(
            @{Field="quotationId"; Value=$response7.quotationId; Expected=$quotationId},
            @{Field="itemType"; Value=$response7.itemType; Expected="AnalysisItem"},
            @{Field="quantity"; Value=$response7.quantity; Expected=1},
            @{Field="unitPrice"; Value=$response7.unitPrice; Expected=200000},
            @{Field="subTotal"; Value=$response7.subTotal; Expected=200000},
            @{Field="itemCode"; Value=$response7.itemCode; Expected="CT-0005"},
            @{Field="itemNameVi"; Value=$response7.itemNameVi; Expected="Sulfadimethoxine"},
            @{Field="itemNameEn"; Value=$response7.itemNameEn; Expected="Sulfadimethoxine"},
            @{Field="defaultPrice"; Value=$response7.defaultPrice; Expected=200000},
            @{Field="discountPercent"; Value=$response7.discountPercent; Expected=0},
            @{Field="discountAmount"; Value=$response7.discountAmount; Expected=0},
            @{Field="displayOrder"; Value=$response7.displayOrder; Expected=1},
            @{Field="analysisItemId"; Value=$response7.analysisItemId; Expected=$analysisItemId},
            @{Field="isStandalone"; Value=$response7.isStandalone; Expected=$true},
            @{Field="sampleMatrixName"; Value=$response7.sampleMatrixName; Expected="Thuc pham"},
            @{Field="publishedGroupCode"; Value=$response7.publishedGroupCode; Expected="VLAB-CH-TP-659"},
            @{Field="unit"; Value=$response7.unit; Expected="ug/kg"},
            @{Field="lod"; Value=$response7.lod; Expected="0.167"},
            @{Field="loq"; Value=$response7.loq; Expected="0.5"},
            @{Field="tat"; Value=$response7.tat; Expected="7"}
        )
        
        foreach ($check in $checks) {
            if ($check.Value -eq $check.Expected) {
                Write-Host "    OK $($check.Field): $($check.Value)" -ForegroundColor Green
            } else {
                Write-Host "    WARNING $($check.Field): $($check.Value) (Expected: $($check.Expected))" -ForegroundColor Yellow
            }
        }
        
    } catch {
        Write-Host "  ERROR: Body day du that bai" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $responseBody = $reader.ReadToEnd()
            Write-Host "  Response: $responseBody" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "=== Test Complete ===" -ForegroundColor Green

} catch {
    Write-Host "FATAL ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

