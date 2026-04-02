# KMD-05-02: Admin – List and Delete Performances

**Epic:** 05 – Performance Management

## Description
Admin can view all performances with filtering options and delete performances that are no longer needed. The list provides an overview of each performance's status including reservation counts.

## Acceptance Criteria
- [ ] Admin can view a list of all performances showing: play name, theatre name, date/time, capacity, reserved count, wave name, season name
- [ ] List is sortable by date (default: upcoming first)
- [ ] List is filterable by season, wave, and theatre
- [ ] Admin can delete a performance only if it has no active reservations
- [ ] Attempting to delete a performance with reservations shows an error with the reservation count
- [ ] A confirmation prompt is shown before deletion
- [ ] Past performances are visually distinguished from upcoming ones
- [ ] Only admins can access these pages

## Technical Notes
- Pages: Admin/Performances/Index, Admin/Performances/Delete
- Show available/total capacity (e.g., "12/30 reserved")
- Consider pagination for large performance lists
- Filtering can use query string parameters for bookmarkable filtered views
