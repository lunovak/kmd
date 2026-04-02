# KMD-07-01: Admin – View Reservations by Season, Performance, and Member

**Epic:** 07 – Admin Reservation Management

## Description
Admin can view all reservations with multiple perspectives: by season (all reservations in a season), by performance (who reserved a specific performance), and by member (all reservations for a specific user). These views support the admin's ability to manage and oversee the reservation system.

## Acceptance Criteria
- [ ] Admin can view all reservations for a selected season in a table showing: member name/email, performance (play + date), status, created date
- [ ] Admin can view all reservations for a specific performance showing: member name/email, status, created date
- [ ] Admin can view all reservations for a specific member showing: performance (play + date + theatre), status, season, created date
- [ ] All views support sorting by date and member name
- [ ] All views show reservation status (Active, Cancelled, Offered) with visual distinction
- [ ] Admin can navigate between views (e.g., click a member name in the performance view to see that member's all reservations)
- [ ] Reservation counts/summaries are shown (e.g., "15 active, 3 cancelled, 2 offered" for a performance)
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Reservations/BySeason, Admin/Reservations/ByPerformance, Admin/Reservations/ByMember
- These are read-only views; write operations are in KMD-07-02
- Use efficient queries with proper includes to avoid N+1 problems
- Consider exporting to CSV for admin convenience (optional, low priority)
