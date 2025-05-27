# Build Fix Summary

## Issues Fixed

1. **Project Reference Structure**
   - Fixed circular dependencies between Application and Infrastructure
   - Updated ElectricityShop.Application to reference ElectricityShop.Domain and ElectricityShop.Infrastructure
   - Updated ElectricityShop.Infrastructure to reference only ElectricityShop.Domain
   - Updated ElectricityShop.API to reference both ElectricityShop.Infrastructure and ElectricityShop.Application

2. **Entity Framework Core Extensions**
   - QueryableExtensions.cs already had proper using directives
   - Code is now correctly using CancellationToken in all async methods
   - EF.CompileQuery methods are properly typed

3. **Expression Tree Issues**
   - ApiWebApplicationFactory.cs was already correctly using helper methods to avoid optional parameters in expression trees

4. **OpenTelemetry Implementation**
   - Added OpenTelemetry packages to the API project (.NET 9 compatible versions)
   - Used release candidate versions where needed to match .NET 9

## Project Structure (After Fixes)

The corrected project reference structure:

```
ElectricityShop.Domain
↑               ↑
|               |
ElectricityShop.Application  <----  ElectricityShop.Infrastructure
↑               ↑
|               |
└───────────  ElectricityShop.API
```

## OpenTelemetry Packages Added

```xml
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Instrumentation.Process" Version="1.7.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.8.0-rc.1" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.8.0-rc.1" />
```

## Next Steps

1. Run `dotnet restore` to restore the new packages
2. Build the solution with `dotnet build`
3. Run the application to test the OpenTelemetry implementation
4. Start the monitoring stack with `docker-compose -f docker-compose.telemetry.yml up -d`
5. Set environment variables for OpenTelemetry:
   ```
   $env:OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:4317"
   $env:OTEL_RESOURCE_ATTRIBUTES = "service.environment=development"
   ```
6. Access monitoring dashboards:
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000 (admin/admin)
