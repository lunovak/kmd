# KMD-01-04: Create Shared Layout, Navigation, and Authorization Setup

**Epic:** 01 – Project Setup & Foundation

## Description
Build the shared Razor Pages layout with role-aware navigation. Configure authorization policies so that all pages require authentication by default, admin pages require the Admin role, and the login page is accessible anonymously. Implement login and logout UI.

## Acceptance Criteria
- [ ] Shared `_Layout.cshtml` with consistent site header, navigation, and footer
- [ ] Navigation shows different menu items based on role (Admin sees admin links, Member sees member links)
- [ ] Login and logout pages work correctly using ASP.NET Core Identity
- [ ] All pages require authentication by default (global authorization filter)
- [ ] Login page is accessible without authentication
- [ ] Admin-only pages are protected with `[Authorize(Roles = "Admin")]`
- [ ] Unauthorized users are redirected to the login page
- [ ] Basic CSS styling for a clean, usable interface (no complex framework required)

## Technical Notes
- Set `options.FallbackPolicy` to require authenticated users globally
- Use `[AllowAnonymous]` only on the login page
- Navigation can use `User.IsInRole("Admin")` in the layout
- Keep the UI simple – Razor Pages with minimal CSS, no JavaScript framework
- Consider using Bootstrap (included by default in ASP.NET templates) for basic styling
