# KMD-04-03: Admin – Manage Member Subscriptions and Season Limits

**Epic:** 04 – Season & Subscription Management

## Description
Admin can assign members to a season via subscriptions and define each member's reservation limit for that season. This is typically done when the season member list is finalized. The subscription limit controls how many total reservations a member can hold across all performances in that season.

## Acceptance Criteria
- [ ] Admin can view all subscriptions for a selected season (member list with their limits)
- [ ] Admin can add a member to a season by creating a subscription with a reservation limit
- [ ] Admin can bulk-create subscriptions for multiple members at once (e.g., select members and set a default limit)
- [ ] Admin can edit an individual member's reservation limit for a season
- [ ] Admin can remove a member's subscription from a season (only if they have no reservations in that season)
- [ ] The subscription list shows each member's current reservation count vs. their limit
- [ ] Form validation: limit must be a positive integer
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Subscriptions/Index (filtered by season), Admin/Subscriptions/Create, Admin/Subscriptions/Edit
- Create a `SubscriptionService` in the Services layer
- Subscription has a composite key: (UserId, SeasonId)
- The reservation count display requires joining with Reservation data (active reservations count)
- Bulk creation can be a multi-select of members with a shared default limit value
