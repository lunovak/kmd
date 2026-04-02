🎟️ Epic 5: Reservation Management
🧾 Reservation Creation

KMD-05-1
As a user, I want to view available performances within a selected reservation wave so that I can choose what to reserve. Just active waves are selectable (those after StartDate and with any Performance happening in future). The one with oldest StartDate is preselected. 

KMD-05-2
As a user, I want to see remaining ticket availability for each performance so that I can decide whether to reserve.

KMD-05-3
As a user, I want to create a reservation for a selected performance so that I can attend it.

KMD-05-4
As a System, I want to validate reservation requests so that:

performance capacity is not exceeded
user’s season limit is not exceeded
wave constraints are respected

KMD-05-5
As a System, I want to assign initial status “Active” to a newly created reservation so that its lifecycle is tracked.

KMD-05-6
As a System, I want to store reservation timestamps (CreateDate, LastUpdateDate) so that changes can be tracked.

📋 Reservation Viewing

KMD-05-7
As a user, I want to view my reservations so that I can track my bookings.

KMD-05-8
As a user, I want to see reservation status so that I understand whether tickets are active or cancelled or offered.

KMD-05-9
As an Admin, I want to view reservations for a specific performance so that I can manage attendance.

KMD-05-10
As an Admin, I want to view reservations for a specific user so that I can monitor usage.

KMD-05-11
As an Admin, I want to view all reservations within a season so that I have an overview of usage.

❌ Reservation Cancellation (Restricted)

KMD-05-12
As a user, I want to cancel my reservation so that I no longer occupy tickets, if it is still more than CanCancelBeforeDays until the performance date.

KMD-05-13
As a System, I want cancellation to change reservation status to Canceled instead of deleting it so that history is preserved.

KMD-05-14
As a System, I want Cancelled reservations show as available so that other users can place new reservation.


🧑‍💼 Admin Reservation Actions

KMD-05-15
As an Admin, I want to create a reservation for a user so that I can assist users.

KMD-05-16
As an Admin, I want to cancel or modify a reservation so that I can resolve issues.

🔍 Reservation Filtering & Context

KMD-05-17
As a user, I want to filter performances by season so that I only see relevant events.

KMD-05-18
As a user, I want to see only reservable performances (based on wave timing) so that I don’t attempt invalid actions.


❌ Reservation Offer (after Cancellation is not allowed)

KMD-05-19
As a user, I want to offer my reservation, when it is less than CanCancelBeforeDays before the performance date, so that other user can contact me. 

KMD-05-20
As a System, I want offering to change the reservation status to Offered.

KMD-05-21
As a System, I want Offered reservations consider as valid, but show the offering meber name so there is a chance to contact him outside of the application.
