# KMD-09-03: Configure Application Insights Logging and Monitoring

**Epic:** 09 – Deployment & Operations

## Description
Integrate Azure Application Insights for application logging, monitoring, and diagnostics. This provides visibility into application health, errors, and usage patterns in production.

## Acceptance Criteria
- [ ] Application Insights SDK is added to the project
- [ ] Instrumentation key / connection string is configurable via environment variables
- [ ] Request logging is enabled (all HTTP requests are tracked)
- [ ] Exceptions and errors are automatically captured
- [ ] Custom log messages from the application (via `ILogger`) flow to Application Insights
- [ ] Application Insights is disabled or uses a lightweight mode in development
- [ ] Key business events are logged at appropriate levels (reservation created, cancelled, email sent, admin actions)

## Technical Notes
- NuGet: `Microsoft.ApplicationInsights.AspNetCore`
- Configure in Program.cs: `builder.Services.AddApplicationInsightsTelemetry()`
- Connection string via `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable
- Use `ILogger<T>` throughout the application – Application Insights picks up logs automatically
- No need for custom telemetry initializers for MVP – default request/exception tracking is sufficient
