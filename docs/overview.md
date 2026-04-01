# Theatre Ticket Reservation App – Overview

## Summary
A private web application for managing theatre ticket reservations within a fixed group of members. An administrator defines performances and available tickets for them in (monthly) waves. Administrator defines performances count limit for each member and season. Members reserve tickets up to the limits. The app is not public-facing.

## Goals for MVP
- Manage a fixed set of users (no public registration)
- Admin can create performances and assign ticket limits
- Members can reserve tickets
- Members can cancel their tickets (make available but still consider reserved until taken by other member)
- Prevent performance overbooking
- Prevent member overbooking
- Send basic email notifications
- Simple, maintainable solution

## Possible goals after MVP phase
- Multi-tenant architecture
- Seat-level selection (only ticket counts for MVP)

## Non-Goals
- Payments or billing
- Public sign-up
- Mobile app or complex frontend

## Core Concepts
- User (Admin or Member, via ASP.NET Identity)
- Performance (event with date/time and ticket capacity)
- Reservation (user reserves N tickets for a performance)

## Functional Requirements

### Authentication
- Use ASP.NET Core Identity
- Users are pre-created by admin upon a season member list
- Roles: Admin, Member

### Admin
- Create/edit/delete performances
- Set ticket capacity
- set user&season limit
- View all season reservations
- View all performance reservations
- View user's reservations
- reserve and unreserve performance for a member
- email season members or performance attendees

### Member
- View performances
- Create/cancel reservations
- View own reservations

### Notifications
- Email on reservation confirmation and important changes
- Email alert on upcoming reserved performance
- Email alert non-attendees if closely upcoming performance is free

## Business Rules
- Reservations are first-come-first-served within the wave reservation phases
- priority phase 1 ticket per wave, later unlimited
- Total reserved tickets cannot exceed performance capacity
- Total member reserved tickets in a season cannot exceed his season limit
- All validation must be enforced server-side

## Technical Stack
- ASP.NET Core Razor Pages
- C#
- Entity Framework Core
- Azure SQL (Free tier)
- ASP.NET Core Identity
- Azure App Service
- Email via Azure Communication Services
- Application Insights logging

## Architecture
- Razor Pages (no MVC unless needed)
- Layers:
  - Pages (UI)
  - Services (business logic)
  - Data (EF Core)
- Use dependency injection
- Keep design simple, avoid over-engineering

## Data Model (High-Level, add more if needed)
- User (Identity, Email, ClassName, OtherContact)
- Season (Id, Name, StartDate)
- ReservationWave (Id, SeasonId, Name, StartDate, PriorityPhaseLengthDays)
- Performance (Id, DateTime, Capacity, ReservationWaveId)
- Reservation (Id, UserId, PerformanceId, Status, CreateDate, LastUpdateDate)
- Theatre (Id, Url, Notes)
- Play (Id, TheatreId, Name, Url, WikiUrl)
- Subscription (UserId, SeasonId, ReservationsLimit)

Relationships:
- Performance → many Reservations
- User → many Reservations
- Theatre → many Performances
- Play → many Performances
- Theatre → many Plays
- Season → many Performances
- User → many Subscriptions
- Season → many Subscriptions

## Security
- Identity for authentication
- Role-based authorization (Admin vs Member)
- Validate all inputs server-side
- Prevent overbooking at database/service level
- No public content
- Keep all user logins cached in memory to avoid accessing database during login page attack

## Deployment
- Azure App Service
- Azure SQL database
- Configuration via environment variables
- CI/CD via GitHub Actions (preferred)

## Development Guidance
- Prefer small, incremental changes
- Keep project always buildable
- Use EF Core migrations for schema changes
- Generate clear, maintainable code (no unnecessary abstractions)
- Avoid introducing new frameworks without clear need

## Out of Scope
- Payments
- Public APIs
- Real-time updates (e.g., SignalR)
- Complex UI frameworks