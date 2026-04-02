# KMD-04-01: Admin – CRUD for Seasons

**Epic:** 04 – Season & Subscription Management

## Description
Admin can create and manage seasons. A season represents a time period (e.g., a theatre season/year) that groups reservation waves and member subscriptions together.

## Acceptance Criteria
- [ ] Admin can view a list of all seasons, ordered by start date (newest first)
- [ ] Admin can create a new season with: Name (required), StartDate (required)
- [ ] Admin can edit an existing season's details
- [ ] Admin can delete a season only if it has no associated waves, performances, or subscriptions
- [ ] The current/active season is visually highlighted in the list
- [ ] Form validation enforced (Name required, StartDate required)
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Seasons/Index, Admin/Seasons/Create, Admin/Seasons/Edit, Admin/Seasons/Delete
- Create a `SeasonService` in the Services layer
- Consider adding an `IsActive` computed property or convention (e.g., latest season is active)
