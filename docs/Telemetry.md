# Performance Monitoring with OpenTelemetry

This document explains how to use and configure OpenTelemetry for performance monitoring in the ElectricityShop API.

## Overview

The ElectricityShop API uses OpenTelemetry for comprehensive performance monitoring, including:

- Request tracking and metrics
- Authentication metrics
- Product metrics
- Order metrics
- Cache metrics
- Business metrics
- System metrics

## Architecture

The monitoring architecture consists of:

1. **Instrumentation in the API**: Using OpenTelemetry SDK to collect metrics and traces
2. **OpenTelemetry Collector**: Receives telemetry data from the API and forwards it to backends
3. **Prometheus**: Stores metrics data
4. **Grafana**: Visualizes the metrics

## Available Metrics

### Request Metrics
- `api.requests.total`: Total number of requests received
- `api.requests.duration`: Duration of requests (in milliseconds)

### Authentication Metrics
- `api.auth.logins.successful`: Number of successful login attempts
- `api.auth.logins.failed`: Number of failed login attempts
- `api.auth.registrations`: Number of user registrations

### Product Metrics
- `api.products.views`: Number of product views
- `api.products.searches`: Number of product searches
- `api.products.creations`: Number of products created
- `api.products.updates`: Number of products updated

### Order Metrics
- `api.orders.created`: Number of orders created
- `api.orders.completed`: Number of orders completed
- `api.orders.cancelled`: Number of orders cancelled

### Cache Metrics
- `api.cache.hits`: Number of cache hits
- `api.cache.misses`: Number of cache misses

### Business Metrics
- `api.business.active_users`: Number of active users in the last 15 minutes
- `api.business.average_order_value`: Average order value in the last 24 hours
- `api.business.product_count`: Total number of active products

### System Metrics
- `api.system.cpu_usage`: Current CPU usage percentage
- `api.system.memory_usage`: Current memory usage in MB

## Usage in Code

To record metrics in your code:

```csharp
// Record a counter increment
ApiMetrics.TotalRequests.Add(1);
ApiMetrics.ProductViews.Add(1);

// Record a request duration
ApiMetrics.RequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds);

// Record an authentication attempt
if (success)
    ApiMetrics.SuccessfulLogins.Add(1);
else
    ApiMetrics.FailedLogins.Add(1);

// Track user activity for active users metric
TelemetryService.RecordUserActivity(userId);

// Record completed order
TelemetryService.RecordOrder(orderAmount);

// Update product count
TelemetryService.UpdateProductCount(count);
```

## Configuration

### Environment Variables

The OpenTelemetry configuration is controlled by the following environment variables:

- `OTEL_EXPORTER_OTLP_ENDPOINT`: The endpoint of the OpenTelemetry Collector (e.g., `http://otel-collector:4317`)
- `OTEL_RESOURCE_ATTRIBUTES`: Additional resource attributes (e.g., `service.environment=production`)

### Configuration in Development

By default, in development environment, metrics are exported to the console. To set up a local monitoring stack:

1. Run the docker-compose file:

```bash
docker-compose -f docker-compose.telemetry.yml up -d
```

2. Set the environment variables:

```bash
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
```

3. Start the API.

### Configuration in Production

In production, set up the following:

1. Deploy the OpenTelemetry Collector, Prometheus, and Grafana using the provided docker-compose file or your infrastructure-as-code solution.

2. Configure the API with the environment variables:

```
OTEL_EXPORTER_OTLP_ENDPOINT=http://<collector-address>:4317
OTEL_RESOURCE_ATTRIBUTES=service.environment=production
```

## Health Checks

A health check for OpenTelemetry is included that verifies:

- The OTLP endpoint is configured
- The connection to the OTLP endpoint is working

The health check endpoint is available at `/health/detail` and can be used to verify that telemetry is working correctly.

## Dashboards

Grafana dashboards are provided for:

- API Overview: Request rate, request duration
- Authentication: Login success/failure rates
- Business Metrics: Active users, average order value

The Grafana UI is available at `http://localhost:3000` when running the docker-compose file.

Default credentials:
- Username: admin
- Password: admin

## Troubleshooting

If metrics are not being collected:

1. Check the API logs for OpenTelemetry-related errors
2. Verify the OTLP endpoint is correctly set
3. Check the OpenTelemetry Collector logs for connection issues
4. Verify Prometheus is scraping the metrics from the collector
5. Check that Grafana can connect to Prometheus

For more detailed diagnostics, enable debug logging for OpenTelemetry by setting:

```
OTEL_LOG_LEVEL=debug
```