# KMD-06-05: Implement Wave Priority Phase Enforcement

**Epic:** 06 – Member Reservation Flow

## Description
Enforce the reservation wave's priority phase rules. During the priority phase (first N days after wave opens), each member can only reserve 1 performance per wave. This ensures fair distribution before allowing unlimited reservations. After the priority phase ends, the restriction is lifted.

## Acceptance Criteria
- [ ] During the priority phase, a member can create at most 1 active reservation in that wave
- [ ] Attempting a second reservation in the same wave during priority phase returns a clear error message including when the priority phase ends
- [ ] After the priority phase (wave.StartDate + PriorityPhaseLengthDays), the per-wave limit is removed
- [ ] Priority phase status is visible to members on the performances list (e.g., "Priority phase – 1 reservation per wave" or "Open – reserve freely")
- [ ] The performances page shows when the priority phase ends for each wave
- [ ] Cancelled reservations during priority phase allow the member to reserve again in that wave
- [ ] Offered reservations still count toward the priority phase limit (member cannot game the system by offering)

## Technical Notes
- This builds on KMD-06-02's reservation logic – may be implemented as part of it or as a refinement
- Priority phase check: `DateTime.UtcNow < wave.StartDate.AddDays(wave.PriorityPhaseLengthDays)`
- Count query: active + offered reservations for this user in performances belonging to this wave
- Display the priority phase end date on the UI: `wave.StartDate.AddDays(wave.PriorityPhaseLengthDays)`
