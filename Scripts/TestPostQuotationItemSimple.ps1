# Script test POST QuotationItem - Đơn giản
# Sử dụng: .\Scripts\TestPostQuotationItemSimple.ps1

$baseUrl = "https://localhost:5001"
$endpoint = "$baseUrl/odata/QuotationItems"

Write-Host "=== Test POST QuotationItem ===" -ForegroundColor Green
Write-Host "Endpoint: $endpoint" -ForegroundColor Cyan
Write-Host ""

# Lấy dữ liệu thực tế từ API trước
Write-Host "Bước 1: Lấy dữ liệu từ database..." -ForegroundColor Yellow

# Bỏ qua SSL certificate check (cho development)
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Lấy Quotation đầu tiên
    $quotations = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations" -Method Get -ContentType "application/json"
    $quotationId = $quotations.value[0].quotationId
    Write-Host "  ✓ QuotationId: $quotationId" -ForegroundColor Green
    
    # Lấy AnalysisItem đầu tiên
    $analysisItems = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems" -Method Get -ContentType "application/json"
    $analysisItemId = $analysisItems.value[0].analysisItemId
    Write-Host "  ✓ AnalysisItemId: $analysisItemId" -ForegroundColor Green
    
    # Lấy Package đầu tiên (nếu có)
    $packages = Invoke-RestMethod -Uri "$baseUrl/odata/Packages" -Method Get -ContentType "application/json"
    $packageId = $null
    if ($packages.value -and $packages.value.Count -gt 0) {
        $packageId = $packages.value[0].packageId
        Write-Host "  ✓ PackageId: $packageId" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Không có Package trong database" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "  ❌ Không thể lấy dữ liệu từ API. Đảm bảo API đang chạy." -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    exit
}

Write-Host ""

# Test POST AnalysisItem Standalone
Write-Host "Bước 2: Test POST AnalysisItem Standalone..." -ForegroundColor Yellow

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

try {
    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -Body $body `
        -Headers @{"Content-Type" = "application/json"} `
        -ContentType "application/json"
    
    Write-Host "✅ SUCCESS - Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host "`n📊 Kiểm tra snapshot tự động:" -ForegroundColor Cyan
    if ($response.itemCode) { Write-Host "  ✓ itemCode: $($response.itemCode)" -ForegroundColor Green }
    if ($response.itemNameVi) { Write-Host "  ✓ itemNameVi: $($response.itemNameVi)" -ForegroundColor Green }
    if ($response.sampleMatrixName) { Write-Host "  ✓ sampleMatrixName: $($response.sampleMatrixName)" -ForegroundColor Green }
    if ($response.publishedGroupCode) { Write-Host "  ✓ publishedGroupCode: $($response.publishedGroupCode)" -ForegroundColor Green }
    if ($response.unit) { Write-Host "  ✓ unit: $($response.unit)" -ForegroundColor Green }
    if ($response.defaultPrice) { Write-Host "  ✓ defaultPrice: $($response.defaultPrice)" -ForegroundColor Green }
    if ($response.tat) { Write-Host "  ✓ tat: $($response.tat)" -ForegroundColor Green }
    
} catch {
    Write-Host "❌ FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Green

