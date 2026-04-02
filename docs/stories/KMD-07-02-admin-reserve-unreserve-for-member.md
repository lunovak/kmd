# KMD-07-02: Admin – Reserve and Unreserve on Behalf of a Member

**Epic:** 07 – Admin Reservation Management

## Description
Admin can create and cancel reservations on behalf of any member. This supports special situations where the admin needs to manually adjust reservations. The same business rules apply but the admin can override certain restrictions (e.g., priority phase limits) when necessary.

## Acceptance Criteria
- [ ] Admin can create a reservation for any active member for any available performance
- [ ] Admin selects the member from a dropdown/search and the performance to reserve
- [ ] Capacity limits are still enforced (admin cannot overbook a performance)
- [ ] Season reservation limits are still enforced unless explicitly overridden
- [ ] Admin can cancel any member's active reservation (both timely and late cancellation rules apply)
- [ ] Admin can choose to force-cancel a reservation without the "Offered" status (full cancellation regardless of deadline)
- [ ] All admin reservation actions are logged (who did what, when)
- [ ] The member receives the same notifications as if they performed the action themselves
- [ ] Only admins can access these pages

## Technical Notes
- Reuse `ReservationService.CreateReservation` and `CancelReservation` with an admin override parameter
- Pages: can be actions within Admin/Reservations/ByPerformance and Admin/Reservations/ByMember views
- Admin override: consider a boolean parameter to skip priority phase check
- Log admin actions using standard logging (Application Insights will capture)
- After admin creates a reservation, trigger the same confirmation email as member self-reservation
