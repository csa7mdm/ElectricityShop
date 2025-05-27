# PowerShell script to generate some load for the API to test telemetry
param(
    [int]$DurationSeconds = 60,
    [int]$RequestsPerSecond = 5,
    [string]$BaseUrl = "https://localhost:5001"
)

Write-Host "Generating load for $DurationSeconds seconds at $RequestsPerSecond requests per second..." -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl" -ForegroundColor Cyan

$endpoints = @(
    @{Method = "GET"; Url = "$BaseUrl/api/TelemetryExample/product/1"; Name = "Get Product 1"},
    @{Method = "GET"; Url = "$BaseUrl/api/TelemetryExample/product/2"; Name = "Get Product 2"},
    @{Method = "GET"; Url = "$BaseUrl/api/TelemetryExample/product/3"; Name = "Get Product 3"},
    @{Method = "GET"; Url = "$BaseUrl/api/TelemetryExample/search?query=test"; Name = "Search Products 'test'"},
    @{Method = "GET"; Url = "$BaseUrl/api/TelemetryExample/search?query=electric"; Name = "Search Products 'electric'"},
    @{Method = "POST"; Url = "$BaseUrl/api/TelemetryExample/login"; Body = @{username = "test"; password = "test"} | ConvertTo-Json; Name = "Login"}
)

$start = Get-Date
$end = $start.AddSeconds($DurationSeconds)
$requestCount = 0
$successCount = 0
$failureCount = 0

# Create a stopwatch for timing
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

while ((Get-Date) -lt $end) {
    $currentSecond = [math]::Floor($stopwatch.Elapsed.TotalSeconds)
    $targetRequests = $currentSecond * $RequestsPerSecond
    
    # Send requests if we're behind the target rate
    while ($requestCount -lt $targetRequests -and (Get-Date) -lt $end) {
        # Pick a random endpoint
        $endpoint = $endpoints[(Get-Random -Minimum 0 -Maximum $endpoints.Length)]
        
        try {
            if ($endpoint.Method -eq "GET") {
                $null = Invoke-RestMethod -Uri $endpoint.Url -Method $endpoint.Method -TimeoutSec 5
            } else {
                $null = Invoke-RestMethod -Uri $endpoint.Url -Method $endpoint.Method -Body $endpoint.Body -ContentType "application/json" -TimeoutSec 5
            }
            $successCount++
            
            if ($requestCount % 10 -eq 0) {
                Write-Host "." -NoNewline -ForegroundColor Green
            }
        }
        catch {
            $failureCount++
            if ($requestCount % 10 -eq 0) {
                Write-Host "×" -NoNewline -ForegroundColor Red
            }
        }
        
        $requestCount++
    }
    
    # Don't hammer the CPU
    Start-Sleep -Milliseconds 10
}

Write-Host ""
Write-Host "Load generation complete!" -ForegroundColor Cyan
Write-Host "Total requests: $requestCount" -ForegroundColor White
Write-Host "Successful requests: $successCount" -ForegroundColor Green
Write-Host "Failed requests: $failureCount" -ForegroundColor Red
Write-Host "Duration: $([math]::Round($stopwatch.Elapsed.TotalSeconds, 2)) seconds" -ForegroundColor White
Write-Host "Average RPS: $([math]::Round($requestCount / $stopwatch.Elapsed.TotalSeconds, 2))" -ForegroundColor White

$stopwatch.Stop()
