# Script GET QuotationItem de xem ten field
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\GetQuotationItemSample.ps1

$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "=== GET QuotationItem Sample ===" -ForegroundColor Green
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/odata/QuotationItems" -Method Get -ContentType "application/json" -ErrorAction Stop
    
    if ($response.value -and $response.value.Count -gt 0) {
        $item = $response.value[0]
        
        Write-Host "Found $($response.value.Count) QuotationItems" -ForegroundColor Cyan
        Write-Host "Sample Item (first one):" -ForegroundColor Yellow
        Write-Host ""
        
        $item | ConvertTo-Json -Depth 10 | Write-Host
        
        Write-Host ""
        Write-Host "=== Field Names Analysis ===" -ForegroundColor Green
        Write-Host ""
        
        # List all field names
        $fieldNames = $item.PSObject.Properties.Name
        Write-Host "All field names:" -ForegroundColor Cyan
        foreach ($fieldName in $fieldNames) {
            $value = $item.$fieldName
            $type = if ($null -eq $value) { "null" } else { $value.GetType().Name }
            Write-Host "  - $fieldName : $type" -ForegroundColor Gray
        }
        
        Write-Host ""
        Write-Host "=== Key Fields ===" -ForegroundColor Green
        Write-Host "  quotationItemId: $($item.quotationItemId)" -ForegroundColor Cyan
        Write-Host "  quotationId: $($item.quotationId)" -ForegroundColor Cyan
        Write-Host "  itemType: $($item.itemType)" -ForegroundColor Cyan
        Write-Host "  analysisItemId: $($item.analysisItemId)" -ForegroundColor Cyan
        Write-Host "  isStandalone: $($item.isStandalone)" -ForegroundColor Cyan
        Write-Host "  defaultPrice: $($item.defaultPrice)" -ForegroundColor Cyan
        Write-Host "  sampleMatrixName: $($item.sampleMatrixName)" -ForegroundColor Cyan
        Write-Host "  publishedGroupCode: $($item.publishedGroupCode)" -ForegroundColor Cyan
        Write-Host "  unit: $($item.unit)" -ForegroundColor Cyan
        Write-Host "  lod: $($item.lod)" -ForegroundColor Cyan
        Write-Host "  loq: $($item.loq)" -ForegroundColor Cyan
        Write-Host "  tat: $($item.tat)" -ForegroundColor Cyan
        
    } else {
        Write-Host "Khong co QuotationItem nao trong database" -ForegroundColor Red
        Write-Host "Tao mot QuotationItem de test..." -ForegroundColor Yellow
        
        # Tao mot QuotationItem de test
        $quotationResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Quotations" -Method Get -ContentType "application/json"
        if ($quotationResponse.value -and $quotationResponse.value.Count -gt 0) {
            $quotationId = $quotationResponse.value[0].quotationId
            
            $analysisItemResponse = Invoke-RestMethod -Uri "$baseUrl/odata/AnalysisItems" -Method Get -ContentType "application/json"
            if ($analysisItemResponse.value -and $analysisItemResponse.value.Count -gt 0) {
                $analysisItemId = $analysisItemResponse.value[0].analysisItemId
                
                $body = @{
                    quotationId = $quotationId
                    itemType = "AnalysisItem"
                    quantity = 1
                    unitPrice = 100000
                    subTotal = 100000
                    analysisItemId = $analysisItemId
                } | ConvertTo-Json
                
                $newItem = Invoke-RestMethod -Uri "$baseUrl/odata/QuotationItems" -Method Post -Body $body -ContentType "application/json"
                Write-Host "Da tao QuotationItem moi: $($newItem.quotationItemId)" -ForegroundColor Green
                Write-Host ""
                $newItem | ConvertTo-Json -Depth 10 | Write-Host
            }
        }
    }
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "Response: $($reader.ReadToEnd())" -ForegroundColor Red
    }
}

