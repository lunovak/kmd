using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class SubscriptionService
{
    private readonly ApplicationDbContext _db;

    public SubscriptionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SubscriptionViewModel>> GetBySeasonAsync(int seasonId)
    {
        var subscriptions = await _db.Subscriptions
            .Include(s => s.User)
            .Where(s => s.SeasonId == seasonId)
            .OrderBy(s => s.User.Email)
            .ToListAsync();

        var userIds = subscriptions.Select(s => s.UserId).ToList();
        var reservationCounts = await _db.Reservations
            .Where(r => r.Status == ReservationStatus.Active
                && r.Performance.ReservationWave.SeasonId == seasonId
                && userIds.Contains(r.UserId))
            .GroupBy(r => r.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        return subscriptions.Select(s => new SubscriptionViewModel
        {
            UserId = s.UserId,
            SeasonId = s.SeasonId,
            UserEmail = s.User.Email!,
            UserClassName = s.User.ClassName,
            ReservationsLimit = s.ReservationsLimit,
            ActiveReservationCount = reservationCounts.GetValueOrDefault(s.UserId, 0)
        }).ToList();
    }

    public async Task<Subscription?> GetAsync(string userId, int seasonId) =>
        await _db.Subscriptions.FindAsync(userId, seasonId);

    public async Task CreateAsync(Subscription subscription)
    {
        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync();
    }

    public async Task BulkCreateAsync(IEnumerable<string> userIds, int seasonId, int defaultLimit)
    {
        foreach (var userId in userIds)
        {
            var exists = await _db.Subscriptions.AnyAsync(s => s.UserId == userId && s.SeasonId == seasonId);
            if (!exists)
            {
                _db.Subscriptions.Add(new Subscription
                {
                    UserId = userId,
                    SeasonId = seasonId,
                    ReservationsLimit = defaultLimit
                });
            }
        }
        await _db.SaveChangesAsync();
    }

    public async Task UpdateLimitAsync(string userId, int seasonId, int newLimit)
    {
        var sub = await _db.Subscriptions.FindAsync(userId, seasonId);
        if (sub != null)
        {
            sub.ReservationsLimit = newLimit;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> CanRemoveAsync(string userId, int seasonId) =>
        !await _db.Reservations.AnyAsync(r =>
            r.UserId == userId
            && r.Status == ReservationStatus.Active
            && r.Performance.ReservationWave.SeasonId == seasonId);

    public async Task RemoveAsync(string userId, int seasonId)
    {
        var sub = await _db.Subscriptions.FindAsync(userId, seasonId);
        if (sub != null)
        {
            _db.Subscriptions.Remove(sub);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<ApplicationUser>> GetUnsubscribedMembersAsync(int seasonId)
    {
        var subscribedUserIds = await _db.Subscriptions
            .Where(s => s.SeasonId == seasonId)
            .Select(s => s.UserId)
            .ToListAsync();

        return await _db.Users
            .Where(u => !subscribedUserIds.Contains(u.Id)
                && (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow))
            .OrderBy(u => u.Email)
            .ToListAsync();
    }
}

public class SubscriptionViewModel
{
    public string UserId { get; set; } = string.Empty;
    public int SeasonId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string? UserClassName { get; set; }
    public int ReservationsLimit { get; set; }
    public int ActiveReservationCount { get; set; }
}
