# Script test POST QuotationItem
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\RunTest.ps1

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5000"
Write-Host "=== Test POST QuotationItem ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host ""

# Bo qua SSL certificate check neu dung HTTPS
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Buoc 1: Lay QuotationId tu database
    Write-Host "Buoc 1: Lay QuotationId tu database..." -ForegroundColor Yellow
    $quotationsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations" -Method Get -ContentType "application/json" -ErrorAction Stop
    if (-not $quotationsResponse.value -or $quotationsResponse.value.Count -eq 0) {
        Write-Host "  ERROR: Khong co Quotation trong database!" -ForegroundColor Red
        Write-Host "  Vui long tao Quotation truoc khi test." -ForegroundColor Yellow
        exit 1
    }
    $quotationId = $quotationsResponse.value[0].quotationId
    Write-Host "  OK QuotationId: $quotationId" -ForegroundColor Green
    Write-Host ""

    # Buoc 2: Lay AnalysisItemId tu database
    Write-Host "Buoc 2: Lay AnalysisItemId tu database..." -ForegroundColor Yellow
    $analysisItemsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems" -Method Get -ContentType "application/json" -ErrorAction Stop
    if (-not $analysisItemsResponse.value -or $analysisItemsResponse.value.Count -eq 0) {
        Write-Host "  ERROR: Khong co AnalysisItem trong database!" -ForegroundColor Red
        Write-Host "  Vui long tao AnalysisItem truoc khi test." -ForegroundColor Yellow
        exit 1
    }
    $analysisItemId = $analysisItemsResponse.value[0].analysisItemId
    $analysisItemCode = $analysisItemsResponse.value[0].analysisItemCode
    $analysisItemName = $analysisItemsResponse.value[0].nameVi
    Write-Host "  OK AnalysisItemId: $analysisItemId" -ForegroundColor Green
    Write-Host "  OK AnalysisItemCode: $analysisItemCode" -ForegroundColor Green
    Write-Host "  OK AnalysisItemName: $analysisItemName" -ForegroundColor Green
    Write-Host ""

    # Buoc 3: POST QuotationItem (AnalysisItem Standalone)
    Write-Host "Buoc 3: POST QuotationItem (AnalysisItem Standalone)..." -ForegroundColor Yellow
    
    $body = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        analysisItemId = $analysisItemId
        isStandalone = $true
        quantity = 1
        unitPrice = 150000
        subTotal = 150000
    } | ConvertTo-Json

    Write-Host "Request Body:" -ForegroundColor Cyan
    Write-Host $body
    Write-Host ""

    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }

    $endpoint = "$baseUrl/odata/QuotationItems"
    Write-Host "POST to: $endpoint" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -Body $body `
        -Headers $headers `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "SUCCESS - QuotationItem da duoc tao!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    Write-Host ""

    # Buoc 4: Kiem tra snapshot tu dong tu master data
    Write-Host "Buoc 4: Kiem tra snapshot tu dong tu master data:" -ForegroundColor Yellow
    $checks = @(
        @{Field="quotationItemId"; Value=$response.quotationItemId; Expected="Co gia tri"},
        @{Field="itemCode"; Value=$response.itemCode; Expected="Tu AnalysisItem.AnalysisItemCode"},
        @{Field="itemNameVi"; Value=$response.itemNameVi; Expected="Tu AnalysisItem.NameVi"},
        @{Field="itemNameEn"; Value=$response.itemNameEn; Expected="Tu AnalysisItem.NameEn"},
        @{Field="sampleMatrixName"; Value=$response.sampleMatrixName; Expected="Tu SampleMatrix.NameVi"},
        @{Field="publishedGroupCode"; Value=$response.publishedGroupCode; Expected="Tu AnalysisItem.PublishedGroupCode"},
        @{Field="unit"; Value=$response.unit; Expected="Tu AnalysisItem.Unit"},
        @{Field="lod"; Value=$response.lod; Expected="Tu AnalysisItem.Lod"},
        @{Field="loq"; Value=$response.loq; Expected="Tu AnalysisItem.Loq"},
        @{Field="tat"; Value=$response.tat; Expected="Tu AnalysisItemTat"},
        @{Field="defaultPrice"; Value=$response.defaultPrice; Expected="Tu AnalysisItem.UnitPrice"}
    )

    foreach ($check in $checks) {
        if ($check.Value) {
            Write-Host "  OK $($check.Field): $($check.Value) ($($check.Expected))" -ForegroundColor Green
        } else {
            Write-Host "  WARNING: $($check.Field): (null) - $($check.Expected)" -ForegroundColor Yellow
        }
    }

    Write-Host ""
    Write-Host "=== Test Complete ===" -ForegroundColor Green
    Write-Host "SUCCESS - QuotationItem da duoc tao thanh cong voi ID: $($response.quotationItemId)" -ForegroundColor Green

} catch {
    Write-Host ""
    Write-Host "TEST FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    
    if ($_.Exception.Response) {
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response Body: $responseBody" -ForegroundColor Red
        } catch {
            Write-Host "Could not read response body" -ForegroundColor Yellow
        }
    }
    
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. Is API running at $baseUrl" -ForegroundColor Yellow
    Write-Host "  2. Does database have Quotation and AnalysisItem data" -ForegroundColor Yellow
    Write-Host "  3. Is connection string in appsettings.json correct" -ForegroundColor Yellow
    Write-Host "  4. Have you run UpdateQuotationItemTable.sql script" -ForegroundColor Yellow
    
    exit 1
}
