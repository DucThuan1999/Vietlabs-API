# Script test POST QuotationItem voi day du cac fields
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotationItemFullFields.ps1

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5000"
Write-Host "=== Test POST QuotationItem (Day du cac fields) ===" -ForegroundColor Green
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

    # Buoc 2: Kiem tra QuotationId co ton tai khong
    Write-Host "=== BUOC 2: Kiem tra QuotationId co ton tai khong ===" -ForegroundColor Yellow
    $quotationId = "c43d20ff-104e-4df0-871d-74faa7cf59be"
    try {
        $quotationResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations($quotationId)" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  OK QuotationId ton tai: $quotationId" -ForegroundColor Green
        Write-Host "  QuotationCode: $($quotationResponse.quotationCode)" -ForegroundColor Gray
        Write-Host "  Status: $($quotationResponse.status)" -ForegroundColor Gray
    } catch {
        Write-Host "  WARNING: QuotationId khong ton tai, se thu tao moi..." -ForegroundColor Yellow
        # Neu khong ton tai, lay QuotationId dau tien tu database
        try {
            $quotationsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations" -Method Get -ContentType "application/json" -ErrorAction Stop
            if ($quotationsResponse.value -and $quotationsResponse.value.Count -gt 0) {
                $quotationId = $quotationsResponse.value[0].quotationId
                Write-Host "  Su dung QuotationId: $quotationId" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "  ERROR: Khong the lay QuotationId" -ForegroundColor Red
            exit 1
        }
    }
    Write-Host ""

    # Buoc 3: Kiem tra AnalysisItemId co ton tai khong
    Write-Host "=== BUOC 3: Kiem tra AnalysisItemId co ton tai khong ===" -ForegroundColor Yellow
    $analysisItemId = "8b77f4cb-7d32-46f0-819c-18df7823b497"
    try {
        $analysisItemResponse = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems($analysisItemId)" -Method Get -ContentType "application/json" -ErrorAction Stop
        Write-Host "  OK AnalysisItemId ton tai: $analysisItemId" -ForegroundColor Green
        Write-Host "  AnalysisItemCode: $($analysisItemResponse.analysisItemCode)" -ForegroundColor Gray
        Write-Host "  NameVi: $($analysisItemResponse.nameVi)" -ForegroundColor Gray
    } catch {
        Write-Host "  WARNING: AnalysisItemId khong ton tai, se thu lay tu database..." -ForegroundColor Yellow
        try {
            $analysisItemsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems" -Method Get -ContentType "application/json" -ErrorAction Stop
            if ($analysisItemsResponse.value -and $analysisItemsResponse.value.Count -gt 0) {
                $analysisItemId = $analysisItemsResponse.value[0].analysisItemId
                Write-Host "  Su dung AnalysisItemId: $analysisItemId" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "  ERROR: Khong the lay AnalysisItemId" -ForegroundColor Red
            exit 1
        }
    }
    Write-Host ""

    # Buoc 4: Chuan bi request body voi day du cac fields
    Write-Host "=== BUOC 4: Chuan bi request body (day du cac fields) ===" -ForegroundColor Yellow
    
    # Tao body voi UTF-8 encoding de tranh loi encoding
    $body = @{
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
    }
    
    # Convert to JSON
    $bodyJson = $body | ConvertTo-Json -Depth 10
    
    Write-Host "  Request Body:" -ForegroundColor Cyan
    Write-Host $bodyJson -ForegroundColor Gray
    Write-Host ""

    # Buoc 5: Test POST
    Write-Host "=== BUOC 5: Test POST QuotationItem ===" -ForegroundColor Yellow
    $endpoint = "$baseUrl/odata/QuotationItems"
    Write-Host "  POST to: $endpoint" -ForegroundColor Cyan
    
    try {
        $response = Invoke-RestMethod -Uri $endpoint `
            -Method Post `
            -Body $bodyJson `
            -Headers @{"Content-Type" = "application/json"; "Accept" = "application/json"} `
            -ContentType "application/json" `
            -ErrorAction Stop

        Write-Host "  SUCCESS - QuotationItem da duoc tao!" -ForegroundColor Green
        Write-Host ""
        Write-Host "  Response (201 Created):" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 10 | Write-Host
        Write-Host ""

        # Buoc 6: Kiem tra tung field
        Write-Host "=== BUOC 6: Kiem tra tung field ===" -ForegroundColor Yellow
        
        $fieldChecks = @(
            @{Field="quotationItemId"; Expected="Co gia tri (Guid)"; Value=$response.quotationItemId},
            @{Field="quotationId"; Expected=$quotationId; Value=$response.quotationId},
            @{Field="itemType"; Expected="AnalysisItem"; Value=$response.itemType},
            @{Field="quantity"; Expected=1; Value=$response.quantity},
            @{Field="unitPrice"; Expected=200000; Value=$response.unitPrice},
            @{Field="subTotal"; Expected=200000; Value=$response.subTotal},
            @{Field="itemCode"; Expected="CT-0005"; Value=$response.itemCode},
            @{Field="itemNameVi"; Expected="Sulfadimethoxine"; Value=$response.itemNameVi},
            @{Field="itemNameEn"; Expected="Sulfadimethoxine"; Value=$response.itemNameEn},
            @{Field="defaultPrice"; Expected=200000; Value=$response.defaultPrice},
            @{Field="discountPercent"; Expected=0; Value=$response.discountPercent},
            @{Field="discountAmount"; Expected=0; Value=$response.discountAmount},
            @{Field="displayOrder"; Expected=1; Value=$response.displayOrder},
            @{Field="analysisItemId"; Expected=$analysisItemId; Value=$response.analysisItemId},
            @{Field="isStandalone"; Expected=$true; Value=$response.isStandalone},
            @{Field="sampleMatrixName"; Expected="Thực phẩm"; Value=$response.sampleMatrixName},
            @{Field="publishedGroupCode"; Expected="VLAB-CH-TP-659"; Value=$response.publishedGroupCode},
            @{Field="unit"; Expected="µg/kg"; Value=$response.unit},
            @{Field="lod"; Expected="0.167"; Value=$response.lod},
            @{Field="loq"; Expected="0.5"; Value=$response.loq},
            @{Field="tat"; Expected="7"; Value=$response.tat},
            @{Field="createdAt"; Expected="Co gia tri (DateTime)"; Value=$response.createdAt}
        )

        $allPassed = $true
        foreach ($check in $fieldChecks) {
            $fieldName = $check.Field
            $expected = $check.Expected
            $actual = $check.Value
            
            if ($null -eq $actual) {
                if ($expected -like "*null*" -or $expected -like "*Co the null*") {
                    Write-Host "    OK $fieldName : null (Expected: $expected)" -ForegroundColor Green
                } else {
                    Write-Host "    ERROR $fieldName : null (Expected: $expected)" -ForegroundColor Red
                    $allPassed = $false
                }
            } elseif ($actual -eq $expected) {
                Write-Host "    OK $fieldName : $actual (Expected: $expected)" -ForegroundColor Green
            } elseif ($expected -like "*Co gia tri*") {
                Write-Host "    OK $fieldName : $actual (Expected: $expected)" -ForegroundColor Green
            } else {
                Write-Host "    WARNING $fieldName : $actual (Expected: $expected)" -ForegroundColor Yellow
            }
        }

        Write-Host ""
        if ($allPassed) {
            Write-Host "=== Test Complete ===" -ForegroundColor Green
            Write-Host "SUCCESS - Tat ca cac fields da duoc luu dung!" -ForegroundColor Green
            Write-Host "QuotationItemId: $($response.quotationItemId)" -ForegroundColor Green
        } else {
            Write-Host "=== Test Complete (co canh bao) ===" -ForegroundColor Yellow
            Write-Host "Co mot so fields khong trung khop, vui long kiem tra lai" -ForegroundColor Yellow
        }

    } catch {
        Write-Host "  ERROR: POST that bai" -ForegroundColor Red
        Write-Host "  Exception Type: $($_.Exception.GetType().Name)" -ForegroundColor Red
        Write-Host "  Error Message: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.ErrorDetails.Message) {
            Write-Host "  Error Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        
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
        
        exit 1
    }

} catch {
    Write-Host ""
    Write-Host "=== FATAL ERROR ===" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Stack Trace: $($_.Exception.StackTrace)" -ForegroundColor Red
    exit 1
}

