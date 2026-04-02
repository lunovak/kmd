# KMD-06-01: Member – View Available Performances

**Epic:** 06 – Member Reservation Flow

## Description
Members can browse upcoming performances that are open for reservation. Performances are grouped by reservation wave and show availability status. Only performances belonging to waves that have started are visible. Members can see which performances they have already reserved.

## Acceptance Criteria
- [ ] Members see a list of upcoming performances grouped by reservation wave
- [ ] Each performance shows: play name (as link), theatre name (as link), date/time, available tickets (capacity minus reserved), wave name
- [ ] Only performances in waves whose StartDate has passed are shown (wave is "open")
- [ ] Performances that are fully booked are visually marked but still visible
- [ ] Performances the member has already reserved are clearly indicated
- [ ] Past performances are hidden from the default view
- [ ] The member's current season reservation count vs. limit is displayed prominently
- [ ] Only authenticated members (and admins) can access this page

## Technical Notes
- Page: Performances/Index (member-facing)
- Query must join Performance → ReservationWave → Season and filter by wave start date
- Available count = Performance.Capacity - count of active reservations
- Member's reservation status requires checking Reservation table for the current user
- Season limit info comes from the Subscription table
