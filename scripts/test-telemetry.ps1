# PowerShell script to test OpenTelemetry setup

# Set environment variables for OpenTelemetry
$env:OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:4317"
$env:OTEL_RESOURCE_ATTRIBUTES = "service.environment=development"
$env:OTEL_LOG_LEVEL = "information"

Write-Host "Starting OpenTelemetry test..." -ForegroundColor Cyan

# Check if Docker is running
try {
    docker info | Out-Null
    Write-Host "✓ Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Start the monitoring stack
Write-Host "Starting monitoring stack (OpenTelemetry Collector, Prometheus, Grafana)..." -ForegroundColor Cyan
docker-compose -f docker-compose.telemetry.yml up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to start monitoring stack" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Monitoring stack started" -ForegroundColor Green
Write-Host "  - OpenTelemetry Collector: http://localhost:8889" -ForegroundColor Gray
Write-Host "  - Prometheus: http://localhost:9090" -ForegroundColor Gray
Write-Host "  - Grafana: http://localhost:3000 (admin/admin)" -ForegroundColor Gray

# Wait for services to be ready
Write-Host "Waiting for services to be ready..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

# Check if API project exists
$apiProject = "ElectricityShop.API"
$apiProjectPath = Join-Path (Get-Location) "src\$apiProject\$apiProject.csproj"

if (-not (Test-Path $apiProjectPath)) {
    Write-Host "✗ API project not found at $apiProjectPath" -ForegroundColor Red
    exit 1
}

Write-Host "✓ API project found" -ForegroundColor Green

# Run the API with OpenTelemetry enabled
Write-Host "Starting API with OpenTelemetry enabled..." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the API when done testing" -ForegroundColor Yellow

try {
    Set-Location (Join-Path (Get-Location) "src\$apiProject")
    dotnet run
}
catch {
    Write-Host "✗ Failed to start API" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
finally {
    # Go back to original directory
    Set-Location (Join-Path (Get-Location) "..\..") 
}

# Test API endpoints that generate telemetry
Write-Host "Testing API endpoints that generate telemetry..." -ForegroundColor Cyan

$baseUrl = "https://localhost:5001"

# Test endpoints
$endpoints = @(
    @{Method = "GET"; Url = "$baseUrl/api/TelemetryExample/product/1"; Name = "Get Product"},
    @{Method = "GET"; Url = "$baseUrl/api/TelemetryExample/search?query=test"; Name = "Search Products"},
    @{Method = "POST"; Url = "$baseUrl/api/TelemetryExample/login"; Body = @{username = "test"; password = "test"} | ConvertTo-Json; Name = "Login"}
)

foreach ($endpoint in $endpoints) {
    Write-Host "Testing $($endpoint.Name)..." -ForegroundColor Cyan
    
    try {
        if ($endpoint.Method -eq "GET") {
            Invoke-RestMethod -Uri $endpoint.Url -Method $endpoint.Method
        } else {
            Invoke-RestMethod -Uri $endpoint.Url -Method $endpoint.Method -Body $endpoint.Body -ContentType "application/json"
        }
        Write-Host "✓ $($endpoint.Name) succeeded" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ $($endpoint.Name) failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "OpenTelemetry test complete!" -ForegroundColor Cyan
Write-Host "Check Grafana at http://localhost:3000 to see the metrics" -ForegroundColor Cyan
Write-Host "Default credentials: admin/admin" -ForegroundColor Cyan
