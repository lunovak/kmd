# KMD-09-01: Configure Azure App Service and Azure SQL Deployment

**Epic:** 09 – Deployment & Operations

## Description
Set up Azure infrastructure for the application: Azure App Service for hosting and Azure SQL Database for data storage. Configure the application to read connection strings and settings from environment variables in production.

## Acceptance Criteria
- [ ] Azure App Service is configured (or documented with setup steps) for hosting the ASP.NET Core app
- [ ] Azure SQL Database is provisioned (free tier) or setup steps are documented
- [ ] Connection string is configured via App Service application settings (not in code)
- [ ] Application starts correctly in the Azure environment
- [ ] EF Core migrations can be applied to the Azure SQL database (via startup migration or deployment step)
- [ ] HTTPS is enforced in production
- [ ] Environment-specific settings (Development vs Production) are properly separated
- [ ] A README or deployment guide documents the setup steps

## Technical Notes
- Use `ASPNETCORE_ENVIRONMENT` set to "Production" in App Service
- Connection string should use `AZURE_SQL_CONNECTIONSTRING` or similar named setting
- Consider whether migrations run at startup (`Database.Migrate()`) or as a separate deployment step
- Free tier Azure SQL has limitations – document them
- Configure CORS if needed (likely not for Razor Pages)
