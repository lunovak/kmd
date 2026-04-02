🔄 Epic 6: Reservation Lifecycle & Ticket Reallocation
🔁 Reservation Status Model

KMD-06-1
As a System, I want reservations to have statuses (Active, Cancelled, Offered) so that lifecycle transitions are tracked.

📤 Cancelling Tickets

KMD-06-2
As a user, I want to timely cancel my reservation so that other users can place a new reservation. I want released tickets to be counted as available capacity so that others can reserve them.

KMD-06-3
As a System, I want cancelled reservations to remain associated with the original user so that ownership history is preserved.

📥 Taking Released Tickets

KMD-06-5
As a user, I want to reserve tickets that were released by another user so that I can attend a performance.

KMD-06-6
As a System, I want taking released tickets to:

create or update a reservation for the new user
update the original reservation status to “Reassigned”
🔄 Ownership Transfer Rules

KMD-06-7
As a System, I want ownership of tickets to be clearly assigned to a single user so that ambiguity is avoided.

KMD-06-8
As a System, I want reassignment to respect all reservation constraints so that limits are not bypassed.

⏱️ Lifecycle Tracking

KMD-06-9
As a System, I want to update LastUpdateDate on every status change so that lifecycle events are traceable.

⚖️ Lifecycle Constraints

KMD-06-10
As a System, I want to prevent invalid state transitions so that:

Active → Reassigned is not allowed directly
Released → Active is not allowed without explicit reassignment
Reassigned reservations cannot be modified
🔍 Visibility of Released Tickets

KMD-06-11
As a user, I want to see available released tickets so that I can take them.

KMD-06-12
As a System, I want released tickets to be included in availability calculations so that capacity is accurate.