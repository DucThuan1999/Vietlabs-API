# Script GET top 10 QuotationItems de xem cac fields
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\GetQuotationItemsTop10.ps1

$baseUrl = "http://localhost:5000"
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "=== GET Top 10 QuotationItems ===" -ForegroundColor Green
Write-Host ""

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/odata/QuotationItems" -Method Get -ContentType "application/json" -ErrorAction Stop
    
    if ($response.value -and $response.value.Count -gt 0) {
        $items = $response.value | Select-Object -First 10
        
        Write-Host "Found $($response.value.Count) QuotationItems" -ForegroundColor Cyan
        Write-Host "Showing top 10:" -ForegroundColor Cyan
        Write-Host ""
        
        # Hien thi danh sach cac fields co trong item dau tien
        $firstItem = $items[0]
        Write-Host "=== Fields co trong QuotationItem ===" -ForegroundColor Yellow
        $fieldNames = $firstItem.PSObject.Properties.Name | Sort-Object
        foreach ($fieldName in $fieldNames) {
            $value = $firstItem.$fieldName
            $displayValue = if ($null -eq $value) { "null" } else { $value.ToString() }
            if ($displayValue.Length -gt 50) {
                $displayValue = $displayValue.Substring(0, 50) + "..."
            }
            Write-Host "  - $fieldName : $displayValue" -ForegroundColor Gray
        }
        
        Write-Host ""
        Write-Host "=== Kiem tra cac fields moi ===" -ForegroundColor Yellow
        
        # Cac fields moi can kiem tra
        $newFields = @(
            "isStandalone",
            "IsStandalone",
            "defaultPrice",
            "DefaultPrice",
            "sampleMatrixName",
            "SampleMatrixName",
            "publishedGroupCode",
            "PublishedGroupCode",
            "unit",
            "Unit",
            "lod",
            "Lod",
            "loq",
            "Loq",
            "tat",
            "Tat"
        )
        
        $foundFields = @()
        foreach ($field in $newFields) {
            if ($firstItem.PSObject.Properties.Name -contains $field) {
                $foundFields += $field
                $value = $firstItem.$field
                Write-Host "  OK $field : $value" -ForegroundColor Green
            }
        }
        
        if ($foundFields.Count -eq 0) {
            Write-Host "  WARNING: Khong tim thay cac fields moi trong response!" -ForegroundColor Red
            Write-Host "  Co the API chua restart hoac Entity Framework chua nhan dien cac columns moi" -ForegroundColor Yellow
        } else {
            Write-Host ""
            Write-Host "  Tim thay $($foundFields.Count) fields moi!" -ForegroundColor Green
        }
        
        Write-Host ""
        Write-Host "=== Top 10 QuotationItems (chi hien thi cac fields quan trong) ===" -ForegroundColor Yellow
        Write-Host ""
        
        $index = 1
        foreach ($item in $items) {
            Write-Host "[$index] QuotationItemId: $($item.quotationItemId -or $item.QuotationItemId)" -ForegroundColor Cyan
            Write-Host "    QuotationId: $($item.quotationId -or $item.QuotationId)" -ForegroundColor Gray
            Write-Host "    ItemType: $($item.itemType -or $item.ItemType)" -ForegroundColor Gray
            Write-Host "    AnalysisItemId: $($item.analysisItemId -or $item.AnalysisItemId)" -ForegroundColor Gray
            Write-Host "    IsStandalone: $($item.isStandalone -or $item.IsStandalone)" -ForegroundColor $(if ($item.isStandalone -or $item.IsStandalone) { "Green" } else { "Gray" })
            Write-Host "    DefaultPrice: $($item.defaultPrice -or $item.DefaultPrice)" -ForegroundColor $(if ($item.defaultPrice -or $item.DefaultPrice) { "Green" } else { "Gray" })
            Write-Host "    SampleMatrixName: $($item.sampleMatrixName -or $item.SampleMatrixName)" -ForegroundColor $(if ($item.sampleMatrixName -or $item.SampleMatrixName) { "Green" } else { "Gray" })
            Write-Host "    PublishedGroupCode: $($item.publishedGroupCode -or $item.PublishedGroupCode)" -ForegroundColor $(if ($item.publishedGroupCode -or $item.PublishedGroupCode) { "Green" } else { "Gray" })
            Write-Host "    Unit: $($item.unit -or $item.Unit)" -ForegroundColor $(if ($item.unit -or $item.Unit) { "Green" } else { "Gray" })
            Write-Host "    Lod: $($item.lod -or $item.Lod)" -ForegroundColor $(if ($item.lod -or $item.Lod) { "Green" } else { "Gray" })
            Write-Host "    Loq: $($item.loq -or $item.Loq)" -ForegroundColor $(if ($item.loq -or $item.Loq) { "Green" } else { "Gray" })
            Write-Host "    Tat: $($item.tat -or $item.Tat)" -ForegroundColor $(if ($item.tat -or $item.Tat) { "Green" } else { "Gray" })
            Write-Host "    ItemCode: $($item.itemCode -or $item.ItemCode)" -ForegroundColor Gray
            Write-Host "    ItemNameVi: $($item.itemNameVi -or $item.ItemNameVi)" -ForegroundColor Gray
            Write-Host "    UnitPrice: $($item.unitPrice -or $item.UnitPrice)" -ForegroundColor Gray
            Write-Host "    SubTotal: $($item.subTotal -or $item.SubTotal)" -ForegroundColor Gray
            Write-Host "    CreatedAt: $($item.createdAt -or $item.CreatedAt)" -ForegroundColor Gray
            Write-Host ""
            $index++
        }
        
        Write-Host "=== Full JSON Response (item dau tien) ===" -ForegroundColor Yellow
        $firstItem | ConvertTo-Json -Depth 10 | Write-Host
        
    } else {
        Write-Host "Khong co QuotationItem nao trong database" -ForegroundColor Red
    }
    
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        Write-Host "Response: $($reader.ReadToEnd())" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "API co the chua khoi dong. Vui long start API va thu lai." -ForegroundColor Yellow
}

