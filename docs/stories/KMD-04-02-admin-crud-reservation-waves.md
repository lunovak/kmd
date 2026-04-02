# KMD-04-02: Admin – CRUD for Reservation Waves

**Epic:** 04 – Season & Subscription Management

## Description
Admin can create and manage reservation waves within a season. Each wave defines when reservations open, how long the priority phase lasts, and the cancellation deadline. Performances are assigned to waves, and waves control the reservation timing and rules.

## Acceptance Criteria
- [ ] Admin can view all reservation waves for a selected season
- [ ] Admin can create a new wave with: Name (required), SeasonId, StartDate (required), PriorityPhaseLengthDays (required), CanCancelBeforeDays (required)
- [ ] Season selection uses a dropdown or the wave is created within a season's detail page
- [ ] Admin can edit an existing wave's details
- [ ] Admin can delete a wave only if it has no associated performances
- [ ] Waves are displayed in chronological order by StartDate
- [ ] Form validation: StartDate must be within or after the season's start date
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Waves/Index (filtered by season), Admin/Waves/Create, Admin/Waves/Edit, Admin/Waves/Delete
- Create a `ReservationWaveService` in the Services layer
- PriorityPhaseLengthDays: during this period from wave start, members can only reserve 1 performance per wave
- CanCancelBeforeDays: members can freely cancel up to this many days before the performance date
