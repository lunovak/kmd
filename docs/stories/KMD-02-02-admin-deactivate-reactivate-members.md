# KMD-02-02: Admin – Deactivate and Reactivate Members

**Epic:** 02 – User Management

## Description
Admin can deactivate members who should no longer access the system and reactivate them if needed. Deactivation prevents login but preserves historical reservation data. Members are never hard-deleted to maintain data integrity.

## Acceptance Criteria
- [ ] Admin can deactivate a member from the member list or detail page
- [ ] Deactivated members cannot log in
- [ ] Deactivated members are visually distinguished in the member list (e.g., grayed out)
- [ ] Admin can reactivate a previously deactivated member
- [ ] Deactivation does not delete the member's existing reservations
- [ ] A confirmation prompt is shown before deactivation
- [ ] Deactivated members are excluded from subscription management and reservation pages by default

## Technical Notes
- Use Identity's `LockoutEnd` property or a custom `IsActive` flag on `ApplicationUser`
- If using `IsActive`, check it during login (custom sign-in logic or claim)
- Update the login cache when activation status changes
- Soft-delete pattern – never remove user records
