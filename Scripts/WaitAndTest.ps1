# Wait for API to start and then run test
$baseUrl = "https://localhost:5001"
$maxAttempts = 30
$attempt = 0

Write-Host "Waiting for API to start at $baseUrl..." -ForegroundColor Yellow

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

while ($attempt -lt $maxAttempts) {
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/odata/Quotations" -Method Get -TimeoutSec 2 -ErrorAction Stop
        Write-Host "API is ready!" -ForegroundColor Green
        Write-Host ""
        break
    } catch {
        $attempt++
        Write-Host "Attempt $attempt/$maxAttempts - API not ready yet, waiting..." -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
}

if ($attempt -ge $maxAttempts) {
    Write-Host "ERROR: API did not start after $maxAttempts attempts" -ForegroundColor Red
    Write-Host "Please start the API manually with: dotnet run" -ForegroundColor Yellow
    exit 1
}

# Run the test
Write-Host "Running test..." -ForegroundColor Cyan
Write-Host ""
& "$PSScriptRoot\RunTest.ps1"

