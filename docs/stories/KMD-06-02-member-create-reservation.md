# KMD-06-02: Member – Create a Reservation

**Epic:** 06 – Member Reservation Flow

## Description
Members can reserve a ticket for a performance. The system enforces all business rules: performance capacity, season reservation limit, wave priority phase (only 1 reservation per wave during priority), and wave availability. All validation is server-side to prevent race conditions and manipulation.

## Acceptance Criteria
- [ ] Member can reserve a ticket for an available performance via a "Reserve" button
- [ ] Reservation is rejected if the performance is fully booked (capacity reached)
- [ ] Reservation is rejected if the member has reached their season reservation limit
- [ ] Confirmation by the user is required when the member wants to place a second reservation where he already has a reservation for this performance
- [ ] More than one reservation for a performance and member is allowed after the priority phase ends
- [ ] During the priority phase (wave StartDate + PriorityPhaseLengthDays), a member can hold only 1 reservation in that wave
- [ ] After the priority phase ends, the wave limit is lifted (unlimited within capacity/season limits)
- [ ] Reservation is rejected if the wave has not started yet
- [ ] On success, a Reservation record is created with Status = Active, CreatedDate = now
- [ ] Clear success/error messages are shown to the member
- [ ] All validation is enforced server-side (not just client-side)
- [ ] Concurrent reservation attempts are handled safely (no overbooking via race conditions)

## Technical Notes
- Create a `ReservationService` in the Services layer with a `CreateReservation` method
- Use a database transaction or optimistic concurrency to prevent race conditions
- Priority phase check: if `DateTime.UtcNow < wave.StartDate + wave.PriorityPhaseLengthDays`, count member's active reservations in that wave
- Service method should return a result object with success/failure and error reason
