using Kmd.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Services;

public class ReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ReminderService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalHours = _configuration.GetValue("Reminders:CheckIntervalHours", 24);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending performance reminders");
            }

            await Task.Delay(TimeSpan.FromHours(intervalHours), stoppingToken);
        }
    }

    private async Task SendRemindersAsync(CancellationToken ct)
    {
        var daysBefore = _configuration.GetValue("Reminders:DaysBeforePerformance", 3);
        var now = DateTime.UtcNow;
        var windowEnd = now.AddDays(daysBefore);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var reservations = await db.Reservations
            .Include(r => r.User)
            .Include(r => r.Performance).ThenInclude(p => p.Play)
            .Include(r => r.Performance).ThenInclude(p => p.Theatre)
            .Where(r => r.Status == ReservationStatus.Active
                && r.ReminderSentDate == null
                && r.Performance.DateTime > now
                && r.Performance.DateTime <= windowEnd)
            .ToListAsync(ct);

        _logger.LogInformation("Found {Count} reservations needing reminders", reservations.Count);

        foreach (var reservation in reservations)
        {
            if (ct.IsCancellationRequested) break;

            var email = reservation.User.Email;
            if (string.IsNullOrEmpty(email)) continue;

            var perf = reservation.Performance;
            var subject = $"Reminder: {perf.Play.Name} on {perf.DateTime:D}";
            var html = $"""
                <h2>Performance Reminder</h2>
                <p>This is a reminder about your upcoming reservation.</p>
                <table>
                    <tr><td><strong>Play:</strong></td><td>{perf.Play.Name}</td></tr>
                    <tr><td><strong>Theatre:</strong></td><td>{perf.Theatre.Name}</td></tr>
                    <tr><td><strong>Date / Time:</strong></td><td>{perf.DateTime:f}</td></tr>
                </table>
                <p>Enjoy the show!</p>
                """;

            var sent = await emailService.SendEmailAsync(email, subject, html);
            if (sent)
            {
                reservation.ReminderSentDate = now;
                _logger.LogInformation("Reminder sent for reservation {Id} to {Email}", reservation.Id, email);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
