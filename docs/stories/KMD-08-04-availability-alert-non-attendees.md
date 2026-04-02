# KMD-08-04: Notify Non-Attendees When Upcoming Performance Has Availability

**Epic:** 08 – Email Notifications

## Description
When a closely upcoming performance still has available tickets, send an email alert to season members who have NOT reserved that performance. This encourages members to fill remaining seats for performances happening soon.

## Acceptance Criteria
- [ ] A background job identifies upcoming performances (within configurable windows, e.g., 7 and 1 days) that have available tickets
- [ ] Members who are subscribed to the season but have NOT reserved the performance receive a notification first
- [ ] All members who are subscribed to the season but have NOT reserved the performance receive a notification when performance upcoming soon (1 day) is still available
- [ ] Members who cancelled or offered their reservation for that performance are included in the notification
- [ ] The email includes: play name, theatre name, performance date/time, number of available tickets
- [ ] Each availability notification is sent only once per performance per member and emailing window (avoid spamming)
- [ ] The availability windows (days before performance) is configurable
- [ ] Only active (not deactivated) members are notified

## Technical Notes
- Can run as part of the same `BackgroundService` as KMD-08-03 or a separate one
- Track sent notifications to avoid duplicates (e.g., a NotificationLog table or flag)
- Query: performances within window → filter those with available capacity → find subscribed members without active reservations
- Be careful with email volume – this could generate many emails for many performances
