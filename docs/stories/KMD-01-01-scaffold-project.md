# KMD-01-01: Scaffold ASP.NET Core Razor Pages Project

**Epic:** 01 – Project Setup & Foundation

## Description
Create the initial ASP.NET Core Razor Pages project with Entity Framework Core and ASP.NET Core Identity configured. Establish the solution structure with clear separation into Pages (UI), Services (business logic), and Data (EF Core) layers. Configure dependency injection and basic project settings.

## Acceptance Criteria
- [ ] ASP.NET Core Razor Pages project is created and builds successfully
- [ ] Entity Framework Core is configured with Azure SQL connection string (from configuration/environment variables)
- [ ] ASP.NET Core Identity is added to the project
- [ ] Project structure has clear separation: Pages/, Services/, Data/ folders
- [ ] Dependency injection is configured in Program.cs
- [ ] appsettings.json contains placeholders for connection string and other settings
- [ ] Project targets the latest stable .NET version
- [ ] A .gitignore appropriate for .NET is in place

## Technical Notes
- Use `Microsoft.AspNetCore.Identity.EntityFrameworkCore` for Identity
- Use `Microsoft.EntityFrameworkCore.SqlServer` as the database provider
- Configure the DbContext to use SQL Server
- Keep Program.cs clean – register services, middleware, and configuration
- No authentication UI yet (just the Identity framework wired up)
