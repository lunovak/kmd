# KMD-08-01: Set Up Email Service with Azure Communication Services

**Epic:** 08 – Email Notifications

## Description
Configure the email sending infrastructure using Azure Communication Services. Create a reusable email service that other parts of the application can use to send emails. The service should be resilient, configurable, and testable.

## Acceptance Criteria
- [ ] An `IEmailService` interface is defined with a `SendEmailAsync` method
- [ ] An implementation using Azure Communication Services SDK is created
- [ ] Connection string / endpoint is configurable via appsettings or environment variables
- [ ] Sender email address is configurable
- [ ] Email sending failures are logged but do not crash the application or block user operations
- [ ] Email service is registered in dependency injection
- [ ] A test/health-check endpoint (admin only) verifies email configuration works
- [ ] HTML email templates are supported (simple Razor-based or string templates)

## Technical Notes
- NuGet: `Azure.Communication.Email`
- Service: `EmailService` in Services layer, implementing `IEmailService`
- Consider using a background queue for email sending to avoid blocking HTTP requests
- Template approach: simple string interpolation or a lightweight Razor templating library
- In development, consider a "no-op" or logging-only email implementation
