# KMD-06-04: Member – Cancel a Reservation

**Epic:** 06 – Member Reservation Flow

## Description
Members can cancel their active reservations subject to cancellation rules. A timely cancellation (before the CanCancelBeforeDays deadline) releases the ticket fully. A late cancellation marks the reservation as "Offered" – the ticket becomes available to others but still counts against the member's season limit until someone else takes it.

## Acceptance Criteria
- [ ] Member can cancel an active reservation for an upcoming performance
- [ ] If cancellation is before the deadline (performance date minus wave's CanCancelBeforeDays): reservation Status → Cancelled, ticket freed, member's season count decreases
- [ ] If cancellation is after the deadline: reservation Status → Offered, ticket becomes available to others, but still counts toward member's season limit
- [ ] Member cannot cancel a reservation for a past performance
- [ ] A confirmation prompt is shown before cancellation, explaining whether it's timely or late
- [ ] The system clearly communicates the cancellation type and its impact on the member's limit
- [ ] After cancellation, the member is redirected to their reservations list with a success message
- [ ] All cancellation rules are enforced server-side

## Technical Notes
- Extend `ReservationService` with a `CancelReservation` method
- Cancellation deadline calculation: `performance.DateTime - TimeSpan.FromDays(wave.CanCancelBeforeDays)`
- Status transitions: Active → Cancelled (timely) or Active → Offered (late)
- When checking available tickets, count only Active reservations (not Offered or Cancelled)
- When checking member's season limit, count Active + Offered reservations
- An "Offered" reservation is resolved when another member reserves the same performance (the offered ticket is released from the original member's limit) – implement this in the reservation creation logic
