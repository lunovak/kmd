# KMD-01-03: Seed Roles and Admin User, Implement Login Caching

**Epic:** 01 – Project Setup & Foundation

## Description
Seed the Admin and Member roles into the database on application startup. Create an initial admin user from configuration. Implement in-memory caching of user login data to protect the database from login page brute-force attacks.

## Acceptance Criteria
- [ ] On startup, the app ensures "Admin" and "Member" roles exist in the database
- [ ] On startup, if no admin user exists, one is created from configuration (email/password from appsettings or env vars)
- [ ] Initial admin password is configurable and must be changed on first login (or documented as a manual step)
- [ ] User login data is cached in memory to avoid database hits during authentication
- [ ] Cache is invalidated/refreshed when user data changes (create, edit, password change)
- [ ] Seeding is idempotent – running multiple times causes no errors or duplicates

## Technical Notes
- Use a hosted service or startup code in Program.cs for seeding
- For login caching, consider a custom `IUserStore` wrapper or `IMemoryCache` layer over Identity's user lookup
- Admin credentials should come from environment variables in production, not hardcoded
- Log warnings if seeding is skipped (e.g., admin already exists)
