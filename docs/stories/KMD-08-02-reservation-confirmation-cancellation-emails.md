# KMD-08-02: Send Reservation Confirmation and Cancellation Emails

**Epic:** 08 – Email Notifications

## Description
Automatically send email notifications when a reservation is created or cancelled. The member receives a confirmation email on successful reservation and a cancellation notice when a reservation is cancelled (by themselves or by admin).

## Acceptance Criteria
- [ ] When a reservation is successfully created, the member receives a confirmation email
- [ ] Confirmation email includes: play name, theatre name, performance date/time, reservation status, .ics and .ical attached
- [ ] When a reservation is cancelled, the member receives a cancellation email, .ics and .ical attached
- [ ] Cancellation email distinguishes between timely cancellation and "offered" status
- [ ] When an admin creates/cancels a reservation on behalf of a member, the same emails are sent
- [ ] Emails are sent asynchronously (do not block the reservation operation)
- [ ] Email failures are logged but do not cause the reservation operation to fail

## Technical Notes
- Hook into `ReservationService.CreateReservation` and `CancelReservation` to trigger emails
- Use the `IEmailService` from KMD-08-01
- Consider using domain events or calling the email service after successful DB commit
- Email content should be clear and concise – no complex formatting needed for MVP
