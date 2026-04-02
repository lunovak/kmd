# KMD-08-05: Admin – Send Bulk Email to Season Members or Performance Attendees

**Epic:** 08 – Email Notifications

## Description
Admin can compose and send a custom email to all members of a season or to all attendees of a specific performance. This supports announcements, schedule changes, and other administrative communication.

## Acceptance Criteria
- [ ] Admin can compose an email with: subject and body text
- [ ] Admin can choose recipients: "All members of season X" or "All attendees of performance Y"
- [ ] For season recipients: all members with an active subscription to the selected season
- [ ] For performance recipients: all members with an active reservation for the selected performance
- [ ] Admin sees a preview of the recipient count before sending
- [ ] A confirmation step is required before sending
- [ ] Emails are sent asynchronously in the background
- [ ] Admin receives a summary after sending (success count, failure count)
- [ ] Only admins can access this functionality

## Technical Notes
- Pages: Admin/Email/Compose (with recipient type selector)
- Reuse `IEmailService` from KMD-08-01
- Use background processing for bulk sends to avoid HTTP timeouts
- Consider rate limiting to respect Azure Communication Services quotas
- Log all admin-initiated emails for audit purposes
