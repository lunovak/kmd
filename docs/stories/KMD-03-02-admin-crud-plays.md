# KMD-03-02: Admin – CRUD for Plays

**Epic:** 03 – Theatre & Play Catalog

## Description
Admin can manage plays linked to theatres. Each play belongs to a theatre and has a name and optional reference URLs. Plays are used when creating performances.

## Acceptance Criteria
- [ ] Admin can view a list of all plays, filterable by theatre
- [ ] Admin can create a new play with: Name (required), TheatreId (required), Url (optional), WikiUrl (optional)
- [ ] Theatre selection uses a dropdown populated from existing theatres
- [ ] Admin can edit an existing play's details
- [ ] Admin can delete a play only if it has no associated performances
- [ ] Attempting to delete a play with performances shows an error message
- [ ] Form validation enforced (Name and Theatre required)
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Plays/Index, Admin/Plays/Create, Admin/Plays/Edit, Admin/Plays/Delete
- Create a `PlayService` in the Services layer
- Include theatre name in the play list for context
- Check for dependent Performances before allowing delete
