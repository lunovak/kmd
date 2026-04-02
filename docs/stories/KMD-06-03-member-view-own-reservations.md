# KMD-06-03: Member – View Own Reservations

**Epic:** 06 – Member Reservation Flow

## Description
Members can view a list of all their reservations across seasons, with clear status indicators. This is the member's personal dashboard showing upcoming and past reservations.

## Acceptance Criteria
- [ ] Member sees a list of all their reservations showing: play name, theatre name, performance date/time, reservation status, wave/season name
- [ ] Reservations are sorted by performance date (upcoming first)
- [ ] Upcoming reservations are visually separated from past ones
- [ ] Each reservation status is clearly displayed (Active, Cancelled, Offered)
- [ ] For active upcoming reservations, that can still be cancelled, a "Cancel" action is available (links to cancellation flow)
- [ ] For active upcoming reservations, that can not be cancelled, a "Offer" action is available (links to offering flow)
- [ ] The page shows the member's season reservation count vs. limit for the current season
- [ ] Only the logged-in member's reservations are shown (no access to other members' data)

## Technical Notes
- Page: MyReservations/Index
- Query filters by current user's Id
- Join Reservation → Performance → Play → Theatre for display data
- Join Reservation → Performance → ReservationWave → Season for season context
- Active reservations for upcoming performances should be visually prominent
