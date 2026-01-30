# Script test POST QuotationItem
# Sử dụng: .\Scripts\TestPostQuotationItem.ps1

$baseUrl = "https://localhost:5001"
$endpoint = "$baseUrl/odata/QuotationItems"

# Lấy token nếu cần (nếu API yêu cầu authentication)
# $token = "YOUR_TOKEN_HERE"

Write-Host "=== Test POST QuotationItem ===" -ForegroundColor Green
Write-Host ""

# Test 1: AnalysisItem Standalone (Tối thiểu)
Write-Host "Test 1: AnalysisItem Standalone (Tối thiểu)" -ForegroundColor Yellow

$body1 = @{
    quotationId = "00000000-0000-0000-0000-000000000001"  # Thay bằng QuotationId thực tế
    itemType = "AnalysisItem"
    analysisItemId = "00000000-0000-0000-0000-000000000002"  # Thay bằng AnalysisItemId thực tế
    isStandalone = $true
    quantity = 1
    unitPrice = 150000
    subTotal = 150000
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Cyan
Write-Host $body1
Write-Host ""

try {
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    # Thêm token nếu cần
    # if ($token) {
    #     $headers["Authorization"] = "Bearer $token"
    # }
    
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body1 -Headers $headers -ContentType "application/json"
    
    Write-Host "Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host "`n✅ Test 1 PASSED" -ForegroundColor Green
} catch {
    Write-Host "❌ Test 1 FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host "`n" + ("=" * 50) + "`n"

# Test 2: AnalysisItem Standalone (Đầy đủ)
Write-Host "Test 2: AnalysisItem Standalone (Đầy đủ)" -ForegroundColor Yellow

$body2 = @{
    quotationId = "00000000-0000-0000-0000-000000000001"
    itemType = "AnalysisItem"
    analysisItemId = "00000000-0000-0000-0000-000000000002"
    isStandalone = $true
    itemCode = "AI-001"
    itemNameVi = "Xét nghiệm A"
    itemNameEn = "Test A"
    sampleMatrixName = "Mẫu nước"
    publishedGroupCode = "TCVN-123"
    unit = "mg/L"
    lod = "0.001"
    loq = "0.005"
    tat = "5 Days"
    quantity = 1
    defaultPrice = 150000
    unitPrice = 150000
    discountPercent = 10
    discountAmount = 15000
    subTotal = 135000
    displayOrder = 1
    notes = "Ghi chú cho item này"
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Cyan
Write-Host $body2
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body2 -Headers $headers -ContentType "application/json"
    
    Write-Host "Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host "`n✅ Test 2 PASSED" -ForegroundColor Green
} catch {
    Write-Host "❌ Test 2 FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host "`n" + ("=" * 50) + "`n"

# Test 3: Package
Write-Host "Test 3: Package" -ForegroundColor Yellow

$body3 = @{
    quotationId = "00000000-0000-0000-0000-000000000001"
    itemType = "Package"
    packageId = "00000000-0000-0000-0000-000000000003"  # Thay bằng PackageId thực tế
    quantity = 1
    unitPrice = 500000
    subTotal = 500000
} | ConvertTo-Json

Write-Host "Request Body:" -ForegroundColor Cyan
Write-Host $body3
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri $endpoint -Method Post -Body $body3 -Headers $headers -ContentType "application/json"
    
    Write-Host "Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    
    Write-Host "`n✅ Test 3 PASSED" -ForegroundColor Green
} catch {
    Write-Host "❌ Test 3 FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Green

