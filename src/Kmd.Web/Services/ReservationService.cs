using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class ReservationService
{
    private readonly ApplicationDbContext _db;
    private readonly ReservationEmailService _emailService;

    public ReservationService(ApplicationDbContext db, ReservationEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<ReservationResult> CreateReservationAsync(string userId, int performanceId, bool adminOverride = false)
    {
        // Use a serializable transaction to prevent race conditions
        await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            var performance = await _db.Performances
                .Include(p => p.ReservationWave).ThenInclude(w => w.Season)
                .Include(p => p.Reservations)
                .FirstOrDefaultAsync(p => p.Id == performanceId);

            if (performance == null)
                return new ReservationResult(false, "Performance not found.");

            var wave = performance.ReservationWave;
            var now = DateTime.UtcNow;

            // Check wave has started
            if (now < wave.StartDate)
                return new ReservationResult(false, "This reservation wave has not started yet.");

            // Check capacity
            var activeCount = performance.Reservations.Count(r => r.Status == ReservationStatus.Active);
            if (activeCount >= performance.Capacity)
                return new ReservationResult(false, "This performance is fully booked.");

            // Check if user already has an active reservation for this performance
            var existingReservation = performance.Reservations
                .FirstOrDefault(r => r.UserId == userId && r.Status == ReservationStatus.Active);
            if (existingReservation != null)
                return new ReservationResult(false, "You already have an active reservation for this performance.");

            // Check season subscription and limit
            var seasonId = wave.SeasonId;
            var subscription = await _db.Subscriptions.FindAsync(userId, seasonId);
            if (subscription == null && !adminOverride)
                return new ReservationResult(false, "You are not subscribed to this season.");

            if (subscription != null)
            {
                var seasonCount = await GetSeasonReservationCountAsync(userId, seasonId);
                if (seasonCount >= subscription.ReservationsLimit)
                    return new ReservationResult(false, $"You have reached your season reservation limit ({subscription.ReservationsLimit}).");
            }

            // Check priority phase (unless admin override)
            if (!adminOverride)
            {
                var priorityEnd = wave.StartDate.AddDays(wave.PriorityPhaseLengthDays);
                if (now < priorityEnd)
                {
                    var waveReservations = await _db.Reservations
                        .CountAsync(r => r.UserId == userId
                            && r.Performance.ReservationWaveId == wave.Id
                            && (r.Status == ReservationStatus.Active || r.Status == ReservationStatus.Offered));

                    if (waveReservations >= 1)
                        return new ReservationResult(false,
                            $"During the priority phase, you can only reserve 1 performance per wave. Priority phase ends {priorityEnd:d}.");
                }
            }

            // Resolve any "Offered" reservations for this performance by this user (release from limit)
            var offeredReservation = performance.Reservations
                .FirstOrDefault(r => r.UserId != userId && r.Status == ReservationStatus.Offered);
            if (offeredReservation != null)
            {
                offeredReservation.Status = ReservationStatus.Cancelled;
                offeredReservation.LastUpdatedDate = now;
            }

            // Create reservation
            var reservation = new Reservation
            {
                UserId = userId,
                PerformanceId = performanceId,
                Status = ReservationStatus.Active,
                CreatedDate = now,
                LastUpdatedDate = now
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            // Load navigation properties and send confirmation email
            var fullReservation = await GetByIdAsync(reservation.Id);
            if (fullReservation != null)
                _emailService.SendConfirmationInBackground(fullReservation);

            return new ReservationResult(true, "Reservation created successfully.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ReservationResult> CancelReservationAsync(string userId, int reservationId, bool forceCancel = false)
    {
        var reservation = await _db.Reservations
            .Include(r => r.User)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Include(r => r.Performance)
                .ThenInclude(p => p.ReservationWave)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
            return new ReservationResult(false, "Reservation not found.");

        if (reservation.UserId != userId && !forceCancel)
            return new ReservationResult(false, "You can only cancel your own reservations.");

        if (reservation.Status != ReservationStatus.Active)
            return new ReservationResult(false, "Only active reservations can be cancelled.");

        if (reservation.Performance.DateTime < DateTime.UtcNow)
            return new ReservationResult(false, "Cannot cancel a reservation for a past performance.");

        var wave = reservation.Performance.ReservationWave;
        var cancelDeadline = reservation.Performance.DateTime.AddDays(-wave.CanCancelBeforeDays);
        var now = DateTime.UtcNow;

        if (forceCancel || now <= cancelDeadline)
        {
            // Timely cancellation
            reservation.Status = ReservationStatus.Cancelled;
            reservation.LastUpdatedDate = now;
            await _db.SaveChangesAsync();
            _emailService.SendCancellationInBackground(reservation, true);
            return new ReservationResult(true, "Reservation cancelled. The ticket has been freed and your season count decreased.", true);
        }
        else
        {
            // Late cancellation - "Offered"
            reservation.Status = ReservationStatus.Offered;
            reservation.LastUpdatedDate = now;
            await _db.SaveChangesAsync();
            _emailService.SendCancellationInBackground(reservation, false);
            return new ReservationResult(true,
                "Reservation offered. The ticket is available to others, but still counts toward your season limit until someone takes it.", false);
        }
    }

    public async Task<int> GetSeasonReservationCountAsync(string userId, int seasonId) =>
        await _db.Reservations
            .CountAsync(r => r.UserId == userId
                && (r.Status == ReservationStatus.Active || r.Status == ReservationStatus.Offered)
                && r.Performance.ReservationWave.SeasonId == seasonId);

    public async Task<Dictionary<int, ReservationStatus?>> GetUserReservationMapAsync(string userId)
    {
        return await _db.Reservations
            .Where(r => r.UserId == userId && r.Status == ReservationStatus.Active)
            .ToDictionaryAsync(r => r.PerformanceId, r => (ReservationStatus?)r.Status);
    }

    public async Task<List<Reservation>> GetUserReservationsAsync(string userId) =>
        await _db.Reservations
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Include(r => r.Performance)
                .ThenInclude(p => p.ReservationWave)
                    .ThenInclude(w => w.Season)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Performance.DateTime)
            .ToListAsync();

    public async Task<Reservation?> GetByIdAsync(int id) =>
        await _db.Reservations
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Include(r => r.Performance)
                .ThenInclude(p => p.ReservationWave)
                    .ThenInclude(w => w.Season)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Reservation>> GetBySeasonAsync(int seasonId) =>
        await _db.Reservations
            .Include(r => r.User)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Include(r => r.Performance)
                .ThenInclude(p => p.ReservationWave)
            .Where(r => r.Performance.ReservationWave.SeasonId == seasonId)
            .OrderBy(r => r.Performance.DateTime)
            .ThenBy(r => r.User.Email)
            .ToListAsync();

    public async Task<List<Reservation>> GetByPerformanceAsync(int performanceId) =>
        await _db.Reservations
            .Include(r => r.User)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Where(r => r.PerformanceId == performanceId)
            .OrderBy(r => r.User.Email)
            .ToListAsync();

    public async Task<List<Reservation>> GetByMemberAsync(string userId) =>
        await _db.Reservations
            .Include(r => r.Performance)
                .ThenInclude(p => p.Play)
            .Include(r => r.Performance)
                .ThenInclude(p => p.Theatre)
            .Include(r => r.Performance)
                .ThenInclude(p => p.ReservationWave)
                    .ThenInclude(w => w.Season)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Performance.DateTime)
            .ToListAsync();
}

public record ReservationResult(bool Success, string Message, bool? IsTimelyCancellation = null);
