using Kmd.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class AvailabilityAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AvailabilityAlertService> _logger;

    public AvailabilityAlertService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<AvailabilityAlertService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalHours = _configuration.GetValue("AvailabilityAlerts:CheckIntervalHours", 24);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendAlertsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending availability alerts");
            }

            await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
        }
    }

    private async Task SendAlertsAsync(CancellationToken ct)
    {
        var earlyDays = _configuration.GetValue("AvailabilityAlerts:EarlyWindowDays", 7);
        var lateDays = _configuration.GetValue("AvailabilityAlerts:LateWindowDays", 1);
        var now = DateTime.UtcNow;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Find upcoming performances with available capacity
        var performances = await db.Performances
            .Include(p => p.Play)
            .Include(p => p.Theatre)
            .Include(p => p.ReservationWave).ThenInclude(w => w.Season)
            .Include(p => p.Reservations)
            .Where(p => p.DateTime > now && p.DateTime <= now.AddDays(earlyDays))
            .ToListAsync(ct);

        foreach (var perf in performances)
        {
            if (ct.IsCancellationRequested) break;

            var activeCount = perf.Reservations.Count(r => r.Status == ReservationStatus.Active);
            var available = perf.Capacity - activeCount;
            if (available <= 0) continue;

            // Determine notification type based on window
            var daysUntil = (perf.DateTime - now).TotalDays;
            var notificationType = daysUntil <= lateDays ? "availability_late" : "availability_early";

            // Get season subscribers
            var seasonId = perf.ReservationWave.SeasonId;
            var subscribers = await db.Subscriptions
                .Where(s => s.SeasonId == seasonId)
                .Select(s => s.UserId)
                .ToListAsync(ct);

            // Exclude members who already have an active reservation for this performance
            var reservedUserIds = perf.Reservations
                .Where(r => r.Status == ReservationStatus.Active)
                .Select(r => r.UserId)
                .ToHashSet();

            // Exclude already notified members for this performance/type
            var alreadyNotified = await db.NotificationLogs
                .Where(n => n.PerformanceId == perf.Id && n.NotificationType == notificationType)
                .Select(n => n.UserId)
                .ToHashSetAsync(ct);

            var eligibleUserIds = subscribers
                .Where(uid => !reservedUserIds.Contains(uid) && !alreadyNotified.Contains(uid))
                .ToList();

            if (eligibleUserIds.Count == 0) continue;

            // Load eligible users (only active)
            var users = await db.Users
                .Where(u => eligibleUserIds.Contains(u.Id) && !u.LockoutEnd.HasValue)
                .ToListAsync(ct);

            _logger.LogInformation(
                "Sending {Type} alerts for {Performance} ({Play}) to {Count} members, {Available} tickets available",
                notificationType, perf.Id, perf.Play.Name, users.Count, available);

            foreach (var user in users)
            {
                if (ct.IsCancellationRequested) break;
                if (string.IsNullOrEmpty(user.Email)) continue;

                var subject = $"Tickets Available: {perf.Play.Name} on {perf.DateTime:D}";
                var html = $"""
                    <h2>Tickets Available</h2>
                    <p>There {(available == 1 ? "is" : "are")} still <strong>{available}</strong> ticket{(available == 1 ? "" : "s")} available for an upcoming performance.</p>
                    <table>
                        <tr><td><strong>Play:</strong></td><td>{perf.Play.Name}</td></tr>
                        <tr><td><strong>Theatre:</strong></td><td>{perf.Theatre.Name}</td></tr>
                        <tr><td><strong>Date / Time:</strong></td><td>{perf.DateTime:f}</td></tr>
                        <tr><td><strong>Available:</strong></td><td>{available} of {perf.Capacity}</td></tr>
                    </table>
                    <p>Log in to reserve your spot!</p>
                    """;

                var sent = await emailService.SendEmailAsync(user.Email, subject, html);
                if (sent)
                {
                    db.NotificationLogs.Add(new NotificationLog
                    {
                        UserId = user.Id,
                        PerformanceId = perf.Id,
                        NotificationType = notificationType,
                        SentDate = now
                    });
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
