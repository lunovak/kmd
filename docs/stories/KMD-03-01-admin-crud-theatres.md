# KMD-03-01: Admin – CRUD for Theatres

**Epic:** 03 – Theatre & Play Catalog

## Description
Admin can manage theatres in the system. Each theatre has a name, URL (website), and optional notes. Theatres serve as a reference catalog for linking plays and performances.

## Acceptance Criteria
- [ ] Admin can view a list of all theatres
- [ ] Admin can create a new theatre with: Name (required), Url (optional), Notes (optional)
- [ ] Admin can edit an existing theatre's details
- [ ] Admin can delete a theatre only if it has no associated plays or performances
- [ ] Attempting to delete a theatre with dependencies shows an error message
- [ ] Form validation enforced (Name required, Url format if provided)
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Theatres/Index, Admin/Theatres/Create, Admin/Theatres/Edit, Admin/Theatres/Delete
- Create a `TheatreService` in the Services layer
- Check for dependent Plays before allowing delete
