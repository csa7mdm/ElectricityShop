# ElectricityShop - Build and Run Instructions

This document outlines the steps to fix the build issues, add OpenTelemetry support, and run the ElectricityShop project.

## 1. Fix Build Issues

### Project Reference Issues

1. Update project references as detailed in `project-references.md`
2. Ensure correct NuGet packages are installed for each project

### Code Fixes Applied

1. **QueryableExtensions.cs**
   - Added missing using directives
   - Updated ToPaginatedListAsync to include CancellationToken
   - Fixed CompileQuery methods to use synchronous versions

2. **GetProductsQueryHandler.cs**
   - Updated to use cancellation token in ToPaginatedListAsync call

3. **ApiWebApplicationFactory.cs**
   - No changes needed as the expression tree issues were already resolved

### Target Framework

Ensure all projects use the correct target framework (net8.0 instead of net9.0).

## 2. Clean and Rebuild

```powershell
# Navigate to solution directory
cd E:\Projects\ElectricityShop

# Delete bin and obj folders
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force

# Restore packages
dotnet restore

# Build the solution
dotnet build
```

## 3. OpenTelemetry Support

The OpenTelemetry implementation has been added with the following components:

### Key Files
- ApiMetrics.cs: Defines all metrics
- TelemetryService.cs: Provides telemetry data
- RequestTelemetryMiddleware.cs: Tracks HTTP requests
- TelemetryExtensions.cs: Configuration methods
- OpenTelemetryHealthCheck.cs: Health check for OpenTelemetry

### NuGet Packages
Install the required OpenTelemetry packages to the ElectricityShop.API project:

```powershell
cd src\ElectricityShop.API
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.Runtime
dotnet add package OpenTelemetry.Instrumentation.Process
dotnet add package OpenTelemetry.Instrumentation.EntityFrameworkCore
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

## 4. Run the Application

### Start the Monitoring Stack
```powershell
docker-compose -f docker-compose.telemetry.yml up -d
```

### Set Environment Variables
```powershell
$env:OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:4317"
$env:OTEL_RESOURCE_ATTRIBUTES = "service.environment=development"
```

### Run the API
```powershell
cd src\ElectricityShop.API
dotnet run
```

### Generate Test Load
```powershell
cd scripts
.\generate-load.ps1
```

### Access Monitoring Dashboards
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000 (admin/admin)

## 5. Testing

Run the integration tests to ensure everything is working:

```powershell
cd tests\ElectricityShop.API.Tests
dotnet test
```

## 6. Documentation

For more detailed information, refer to:
- docs/Telemetry.md: Comprehensive documentation
- src/ElectricityShop.API/Infrastructure/Telemetry/README.md: Quick reference

## Troubleshooting

If issues persist:
1. Check the logs for detailed error messages
2. Ensure all projects have the correct references
3. Verify NuGet package versions are compatible
4. Make sure the target framework is correct in all projects
