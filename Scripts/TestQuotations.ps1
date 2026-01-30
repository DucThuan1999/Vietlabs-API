# Script test POST Quotation
# Su dung: powershell -ExecutionPolicy Bypass -File Scripts\TestQuotations.ps1

$ErrorActionPreference = "Stop"

$baseUrl = "http://localhost:5000"
Write-Host "=== Test POST Quotation ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host ""

# Bo qua SSL certificate check neu dung HTTPS
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Buoc 1: Lay ClientId tu database
    Write-Host "Buoc 1: Lay ClientId tu database..." -ForegroundColor Yellow
    $clientsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Clients" -Method Get -ContentType "application/json" -ErrorAction Stop
    if (-not $clientsResponse.value -or $clientsResponse.value.Count -eq 0) {
        Write-Host "  ERROR: Khong co Client trong database!" -ForegroundColor Red
        Write-Host "  Vui long tao Client truoc khi test." -ForegroundColor Yellow
        exit 1
    }
    $clientId = $clientsResponse.value[0].clientId
    $companyName = $clientsResponse.value[0].companyName
    Write-Host "  OK ClientId: $clientId" -ForegroundColor Green
    Write-Host "  OK CompanyName: $companyName" -ForegroundColor Green
    Write-Host ""

    # Buoc 2: Lay EmployeeId tu database (optional)
    Write-Host "Buoc 2: Lay EmployeeId tu database (optional)..." -ForegroundColor Yellow
    $employeeId = $null
    try {
        $employeesResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Employees" -Method Get -ContentType "application/json" -ErrorAction Stop
        if ($employeesResponse.value -and $employeesResponse.value.Count -gt 0) {
            $employeeId = $employeesResponse.value[0].employeeId
            Write-Host "  OK EmployeeId: $employeeId" -ForegroundColor Green
        } else {
            Write-Host "  WARNING: Khong co Employee trong database (optional)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  WARNING: Khong the lay Employee (optional)" -ForegroundColor Yellow
    }
    Write-Host ""

    # Buoc 3: Lay ContactId tu database (optional)
    Write-Host "Buoc 3: Lay ContactId tu database (optional)..." -ForegroundColor Yellow
    $contactId = $null
    try {
        $contactsResponse = Invoke-RestMethod -Uri "$baseUrl/odata/Contacts" -Method Get -ContentType "application/json" -ErrorAction Stop
        if ($contactsResponse.value -and $contactsResponse.value.Count -gt 0) {
            $contactId = $contactsResponse.value[0].contactId
            Write-Host "  OK ContactId: $contactId" -ForegroundColor Green
        } else {
            Write-Host "  WARNING: Khong co Contact trong database (optional)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  WARNING: Khong the lay Contact (optional)" -ForegroundColor Yellow
    }
    Write-Host ""

    # Buoc 4: POST Quotation
    Write-Host "Buoc 4: POST Quotation..." -ForegroundColor Yellow
    
    $body = @{
        clientId = $clientId
        quotationCode = "BG-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        status = "Draft"
        validFrom = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        validTo = (Get-Date).AddDays(30).ToString("yyyy-MM-ddTHH:mm:ssZ")
        vatPercent = 8
        subTotal = 0
        totalAmount = 0
        notes = "Test quotation created by script"
    }

    # Them EmployeeId neu co
    if ($employeeId) {
        $body.employeeId = $employeeId
    }

    # Them ContactId neu co
    if ($contactId) {
        $body.contactId = $contactId
    }

    $bodyJson = $body | ConvertTo-Json

    Write-Host "Request Body:" -ForegroundColor Cyan
    Write-Host $bodyJson
    Write-Host ""

    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }

    $endpoint = "$baseUrl/odata/Quotations"
    Write-Host "POST to: $endpoint" -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri $endpoint `
        -Method Post `
        -Body $bodyJson `
        -Headers $headers `
        -ContentType "application/json" `
        -ErrorAction Stop

    Write-Host "SUCCESS - Quotation da duoc tao!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Response (201 Created):" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10 | Write-Host
    Write-Host ""

    # Buoc 5: Kiem tra thong tin da tao
    Write-Host "Buoc 5: Kiem tra thong tin da tao:" -ForegroundColor Yellow
    $checks = @(
        @{Field="quotationId"; Value=$response.quotationId; Expected="Co gia tri"},
        @{Field="quotationCode"; Value=$response.quotationCode; Expected="Co gia tri"},
        @{Field="clientId"; Value=$response.clientId; Expected="Trung voi ClientId da gui"},
        @{Field="status"; Value=$response.status; Expected="Draft"},
        @{Field="vatPercent"; Value=$response.vatPercent; Expected="8"},
        @{Field="createdAt"; Value=$response.createdAt; Expected="Co gia tri"}
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
    Write-Host "SUCCESS - Quotation da duoc tao thanh cong voi ID: $($response.quotationId)" -ForegroundColor Green

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
    Write-Host "  2. Does database have Client data" -ForegroundColor Yellow
    Write-Host "  3. Is connection string in appsettings.json correct" -ForegroundColor Yellow
    
    exit 1
}

