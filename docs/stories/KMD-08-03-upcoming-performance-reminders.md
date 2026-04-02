# KMD-08-03: Send Upcoming Performance Reminder Emails

**Epic:** 08 – Email Notifications

## Description
Send reminder emails to members who have active reservations for performances happening soon. This is a scheduled/batch operation that runs periodically (e.g., daily) and notifies members about their upcoming performances.

## Acceptance Criteria
- [ ] A background job or scheduled task sends reminder emails for upcoming performances
- [ ] Reminders are sent a configurable number of days before the performance (e.g., 3 days before)
- [ ] Only members with active reservations for the performance receive the reminder
- [ ] Reminder email includes: play name, theatre name, performance date/time
- [ ] Each reminder is sent only once per reservation (track whether reminder was sent)
- [ ] The reminder schedule is configurable via appsettings
- [ ] Job failures are logged and do not affect other reminders

## Technical Notes
- Implement as an ASP.NET Core `IHostedService` or `BackgroundService` with a timer
- Add a `ReminderSent` boolean flag to the Reservation entity (or a separate tracking table)
- Query: find active reservations where performance date is within reminder window and reminder not yet sent
- Run the check once daily (configurable interval)
- Consider making the reminder days configurable per wave or globally
