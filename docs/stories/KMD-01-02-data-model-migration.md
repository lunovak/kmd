# KMD-01-02: Implement Data Model and Initial Migration

**Epic:** 01 – Project Setup & Foundation

## Description
Create all Entity Framework Core entity classes and the DbContext configuration reflecting the full data model. Generate the initial EF Core migration. This covers: User (extending IdentityUser), Season, ReservationWave, Performance, Reservation, Theatre, Play, and Subscription.

## Acceptance Criteria
- [ ] `ApplicationUser` entity extends `IdentityUser` with fields: ClassName, OtherContact
- [ ] `Season` entity with: Id, Name, StartDate
- [ ] `ReservationWave` entity with: Id, SeasonId, Name, StartDate, PriorityPhaseLengthDays, CanCancelBeforeDays
- [ ] `Performance` entity with: Id, DateTime, Capacity, ReservationWaveId, PlayId, TheatreId
- [ ] `Reservation` entity with: Id, UserId, PerformanceId, Status, CreatedDate, LastUpdatedDate
- [ ] `Theatre` entity with: Id, Name, Url, Notes
- [ ] `Play` entity with: Id, TheatreId, Name, Url, WikiUrl
- [ ] `Subscription` entity with: UserId, SeasonId, ReservationsLimit (composite key)
- [ ] All relationships and foreign keys are configured (via Fluent API or data annotations)
- [ ] Reservation.Status uses an enum (e.g., Active, Cancelled, Offered)
- [ ] Initial EF Core migration is generated and can be applied to an empty database
- [ ] Appropriate indexes are defined (e.g., on foreign keys, UserId+PerformanceId uniqueness)

## Technical Notes
- Relationships:
  - Performance → many Reservations
  - User → many Reservations
  - Theatre → many Performances (through Play or directly)
  - Play → many Performances
  - Theatre → many Plays
  - Season → many ReservationWaves → many Performances
  - User → many Subscriptions
  - Season → many Subscriptions
- Use `int` for capacity fields
- Consider adding a unique constraint on (UserId, PerformanceId) for Reservation to prevent duplicates
- Keep the DbContext in the Data/ folder
- Entity configurations can use separate IEntityTypeConfiguration classes if complex
