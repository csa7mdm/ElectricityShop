# OpenTelemetry Implementation for ElectricityShop API

This directory contains the OpenTelemetry implementation for performance monitoring in the ElectricityShop API.

## Components

- **ApiMetrics.cs**: Contains all metric definitions for the application
- **TelemetryService.cs**: Provides services for collecting and managing telemetry data
- **RequestTelemetryMiddleware.cs**: Middleware for tracking HTTP requests
- **TelemetryExtensions.cs**: Extension methods for configuring OpenTelemetry

## Setup

1. Add the required NuGet packages listed in the OpenTelemetryPackages.txt file at the root of the API project.
2. Configure the OpenTelemetry services in the Program.cs file.
3. Use the RequestTelemetryMiddleware in the application pipeline.
4. Set the OTEL_EXPORTER_OTLP_ENDPOINT environment variable to point to your OpenTelemetry Collector (e.g., "http://localhost:4317").

## Usage

To record metrics in your code, use the ApiMetrics static class:

```csharp
// Record a counter increment
ApiMetrics.TotalRequests.Add(1);
ApiMetrics.ProductViews.Add(1);

// Record a request duration
ApiMetrics.RequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds);
```

To record business metrics, use the TelemetryService:

```csharp
// Track user activity for active users metric
TelemetryService.RecordUserActivity(userId);

// Record completed order
TelemetryService.RecordOrder(orderAmount);

// Update product count
TelemetryService.UpdateProductCount(count);
```

## Testing

1. Start the monitoring stack using the docker-compose.telemetry.yml file.
2. Run the API with the appropriate environment variables.
3. Use the scripts in the scripts directory to test the telemetry setup and generate load.

## Further Documentation

See the [Telemetry.md](../../../docs/Telemetry.md) file for detailed documentation on using and configuring OpenTelemetry in this application.
