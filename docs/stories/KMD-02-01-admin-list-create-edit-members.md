# KMD-02-01: Admin – List, Create, and Edit Members

**Epic:** 02 – User Management

## Description
Admin can view a list of all members, create new member accounts, and edit existing member details. Members are pre-created by the admin (no self-registration). Each member gets the "Member" role automatically.

## Acceptance Criteria
- [ ] Admin can view a paginated list of all members showing: email, name/className, active status
- [ ] Admin can create a new member by providing: email, className, otherContact, and a temporary password
- [ ] Newly created members are automatically assigned the "Member" role
- [ ] Admin can edit a member's className, otherContact, and email
- [ ] Admin can reset a member's password
- [ ] Form validation is enforced (required fields, email format)
- [ ] All operations are server-side validated
- [ ] Success/error messages are displayed after operations
- [ ] Only admins can access these pages

## Technical Notes
- Use ASP.NET Core Identity's `UserManager<ApplicationUser>` for user operations
- Create a `UserService` in the Services layer to encapsulate user management logic
- Pages: Admin/Members/Index, Admin/Members/Create, Admin/Members/Edit
- Remember to invalidate the login cache (from KMD-01-03) when creating or editing users
