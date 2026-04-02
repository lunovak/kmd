# KMD-05-01: Admin – Create and Edit Performances

**Epic:** 05 – Performance Management

## Description
Admin can create new performances and edit existing ones. Each performance is associated with a play, theatre (derived from the play), and a reservation wave. The admin sets the date/time and ticket capacity. Performances are the core bookable events in the system.

## Acceptance Criteria
- [ ] Admin can create a new performance with: PlayId (required), DateTime (required), Capacity (required, positive integer), ReservationWaveId (required)
- [ ] Play selection dropdown shows play name and theatre name for clarity
- [ ] Wave selection dropdown is filtered by season (admin selects season first, then wave)
- [ ] Theatre is automatically determined from the selected play
- [ ] Admin can edit a performance's DateTime, Capacity, and Wave assignment
- [ ] Capacity cannot be reduced below the current number of active reservations
- [ ] Form validation: DateTime must be in the future (for new performances), Capacity > 0
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Performances/Create, Admin/Performances/Edit
- Create a `PerformanceService` in the Services layer
- Use cascading dropdowns: Season → Wave, Theatre → Play (or flat with search)
- Capacity validation against existing reservations must be server-side enforced
