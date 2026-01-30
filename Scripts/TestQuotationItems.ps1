# Script test POST QuotationItem - Chi tiet tung buoc
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotationItems.ps1

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5000"
Write-Host "=== Test POST QuotationItem (Chi tiet tung buoc) ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host ""

# Bo qua SSL certificate check neu dung HTTPS
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Buoc 1: Kiem tra API co chay khong
    Write-Host "=== BUOC 1: Kiem tra API co chay khong ===" -ForegroundColor Yellow
    try {
        $healthCheck = Invoke-RestMethod -Uri "$baseUrl/odata" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  OK API dang chay" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: API khong chay tai $baseUrl" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""

    # Buoc 2: Lay QuotationId tu database
    Write-Host "=== BUOC 2: Lay QuotationId tu database ===" -ForegroundColor Yellow
    try {
        $quotationsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  Response structure: $($quotationsResponse | ConvertTo-Json -Depth 2)" -ForegroundColor Gray
        
        if (-not $quotationsResponse.value -or $quotationsResponse.value.Count -eq 0) {
            Write-Host "  ERROR: Khong co Quotation trong database!" -ForegroundColor Red
            Write-Host "  Vui long tao Quotation truoc khi test." -ForegroundColor Yellow
            Write-Host "  Chay: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotations.ps1" -ForegroundColor Yellow
            exit 1
        }
        $quotationId = $quotationsResponse.value[0].quotationId
        Write-Host "  OK QuotationId: $quotationId" -ForegroundColor Green
        Write-Host "  OK So luong Quotation: $($quotationsResponse.value.Count)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: Khong the lay Quotation" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""

    # Buoc 3: Lay AnalysisItemId tu database
    Write-Host "=== BUOC 3: Lay AnalysisItemId tu database ===" -ForegroundColor Yellow
    try {
        $analysisItemsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  Response structure: $($analysisItemsResponse | ConvertTo-Json -Depth 2)" -ForegroundColor Gray
        
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
        Write-Host "  OK So luong AnalysisItem: $($analysisItemsResponse.value.Count)" -ForegroundColor Green
    } catch {
        Write-Host "  ERROR: Khong the lay AnalysisItem" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
    Write-Host ""

    # Buoc 4: Kiem tra endpoint QuotationItems co ton tai khong
    Write-Host "=== BUOC 4: Kiem tra endpoint QuotationItems ===" -ForegroundColor Yellow
    try {
        $quotationItemsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/QuotationItems" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  OK Endpoint QuotationItems ton tai" -ForegroundColor Green
        Write-Host "  OK So luong QuotationItem hien tai: $($quotationItemsResponse.value.Count)" -ForegroundColor Green
    } catch {
        Write-Host "  WARNING: Khong the truy cap endpoint QuotationItems" -ForegroundColor Yellow
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Yellow
    }
    Write-Host ""

    # Buoc 5: Chuan bi request body - Test voi body toi thieu
    Write-Host "=== BUOC 5: Chuan bi request body (toi thieu) ===" -ForegroundColor Yellow
    $bodyMinimal = @{
        quotationId = $quotationId
        itemType = "AnalysisItem"
        analysisItemId = $analysisItemId
        quantity = 1
        unitPrice = 150000
        subTotal = 150000
    }
    
    Write-Host "  Body toi thieu:" -ForegroundColor Cyan
    $bodyMinimalJson = $bodyMinimal | ConvertTo-Json
    Write-Host $bodyMinimalJson -ForegroundColor Gray
    Write-Host ""

    # Buoc 6: Test POST voi body toi thieu
    Write-Host "=== BUOC 6: Test POST voi body toi thieu ===" -ForegroundColor Yellow
    $endpoint = "$baseUrl/odata/QuotationItems"
    Write-Host "  POST to: $endpoint" -ForegroundColor Cyan
    
    try {
        $response = Invoke-RestMethod -Uri $endpoint `
            -Method Post `
            -Body $bodyMinimalJson `
            -Headers @{"Content-Type" = "application/json"; "Accept" = "application/json"} `
            -ContentType "application/json" `
            -ErrorAction Stop

        Write-Host "  SUCCESS - QuotationItem da duoc tao!" -ForegroundColor Green
        Write-Host ""
        Write-Host "  Response (201 Created):" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 10 | Write-Host
        Write-Host ""
        
        # Kiem tra cac truong quan trong
        Write-Host "  Kiem tra cac truong:" -ForegroundColor Yellow
        if ($response.quotationItemId) {
            Write-Host "    OK quotationItemId: $($response.quotationItemId)" -ForegroundColor Green
        } else {
            Write-Host "    ERROR: quotationItemId bi null" -ForegroundColor Red
        }
        if ($response.quotationId -eq $quotationId) {
            Write-Host "    OK quotationId: $($response.quotationId)" -ForegroundColor Green
        } else {
            Write-Host "    ERROR: quotationId khong trung khop" -ForegroundColor Red
        }
        if ($response.itemType -eq "AnalysisItem") {
            Write-Host "    OK itemType: $($response.itemType)" -ForegroundColor Green
        } else {
            Write-Host "    ERROR: itemType khong dung" -ForegroundColor Red
        }
        
        Write-Host ""
        Write-Host "=== Test Complete ===" -ForegroundColor Green
        Write-Host "SUCCESS - QuotationItem da duoc tao thanh cong voi ID: $($response.quotationItemId)" -ForegroundColor Green
        exit 0
        
    } catch {
        Write-Host "  ERROR: POST that bai" -ForegroundColor Red
        Write-Host "  Exception Type: $($_.Exception.GetType().Name)" -ForegroundColor Red
        Write-Host "  Error Message: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.ErrorDetails.Message) {
            Write-Host "  Error Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        
        # Doc response body neu co
        if ($_.Exception.Response) {
            try {
                $stream = $_.Exception.Response.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $responseBody = $reader.ReadToEnd()
                Write-Host ""
                Write-Host "  Response Body:" -ForegroundColor Red
                Write-Host $responseBody -ForegroundColor Red
                
                # Parse JSON neu co the
                try {
                    $errorJson = $responseBody | ConvertFrom-Json
                    Write-Host ""
                    Write-Host "  Parsed Error:" -ForegroundColor Red
                    if ($errorJson.errors) {
                        Write-Host "    Errors:" -ForegroundColor Red
                        $errorJson.errors | ConvertTo-Json -Depth 10 | Write-Host
                    }
                    if ($errorJson.title) {
                        Write-Host "    Title: $($errorJson.title)" -ForegroundColor Red
                    }
                    if ($errorJson.status) {
                        Write-Host "    Status: $($errorJson.status)" -ForegroundColor Red
                    }
                } catch {
                    Write-Host "  Khong the parse JSON error" -ForegroundColor Yellow
                }
            } catch {
                Write-Host "  Khong the doc response body" -ForegroundColor Yellow
            }
        }
        
        Write-Host ""
        Write-Host "=== BUOC 7: Test voi body day du ===" -ForegroundColor Yellow
        Write-Host "  Thu lai voi body day du hon..." -ForegroundColor Yellow
        
        # Thu voi body day du hon
        $bodyFull = @{
            quotationId = $quotationId
            itemType = "AnalysisItem"
            analysisItemId = $analysisItemId
            isStandalone = $true
            quantity = 1
            unitPrice = 150000
            subTotal = 150000
            itemCode = $analysisItemCode
            itemNameVi = $analysisItemName
        }
        
        $bodyFullJson = $bodyFull | ConvertTo-Json
        Write-Host "  Body day du:" -ForegroundColor Cyan
        Write-Host $bodyFullJson -ForegroundColor Gray
        Write-Host ""
        
        try {
            $response2 = Invoke-RestMethod -Uri $endpoint `
                -Method Post `
                -Body $bodyFullJson `
                -Headers @{"Content-Type" = "application/json"; "Accept" = "application/json"} `
                -ContentType "application/json" `
                -ErrorAction Stop

            Write-Host "  SUCCESS voi body day du!" -ForegroundColor Green
            Write-Host "  Response:" -ForegroundColor Green
            $response2 | ConvertTo-Json -Depth 10 | Write-Host
            exit 0
            
        } catch {
            Write-Host "  ERROR: Van that bai voi body day du" -ForegroundColor Red
            Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
            if ($_.Exception.Response) {
                try {
                    $stream = $_.Exception.Response.GetResponseStream()
                    $reader = New-Object System.IO.StreamReader($stream)
                    $responseBody2 = $reader.ReadToEnd()
                    Write-Host "  Response Body:" -ForegroundColor Red
                    Write-Host $responseBody2 -ForegroundColor Red
                } catch {}
            }
        }
        
        Write-Host ""
        Write-Host "=== Debugging Tips ===" -ForegroundColor Yellow
        Write-Host "  1. Kiem tra Model QuotationItem co validation attributes khong" -ForegroundColor Yellow
        Write-Host "  2. Kiem tra Controller co [FromBody] dung khong" -ForegroundColor Yellow
        Write-Host "  3. Kiem tra OData configuration" -ForegroundColor Yellow
        Write-Host "  4. Kiem tra database schema co dung khong" -ForegroundColor Yellow
        
        exit 1
    }

} catch {
    Write-Host ""
    Write-Host "=== FATAL ERROR ===" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack Trace: $($_.Exception.StackTrace)" -ForegroundColor Red
    exit 1
}

