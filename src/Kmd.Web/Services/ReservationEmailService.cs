using Kmd.Web.Data;
using System.Text;

namespace Kmd.Web.Services;

public class ReservationEmailService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ReservationEmailService> _logger;

    public ReservationEmailService(IEmailService emailService, ILogger<ReservationEmailService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public void SendConfirmationInBackground(Reservation reservation)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await SendConfirmationAsync(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email for reservation {Id}", reservation.Id);
            }
        });
    }

    public void SendCancellationInBackground(Reservation reservation, bool isTimelyCancellation)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await SendCancellationAsync(reservation, isTimelyCancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send cancellation email for reservation {Id}", reservation.Id);
            }
        });
    }

    private async Task SendConfirmationAsync(Reservation reservation)
    {
        var perf = reservation.Performance;
        var play = perf.Play;
        var theatre = perf.Theatre;
        var email = reservation.User.Email;

        if (string.IsNullOrEmpty(email)) return;

        var subject = $"Reservation Confirmed: {play.Name}";
        var html = $"""
            <h2>Reservation Confirmed</h2>
            <p>Your reservation has been confirmed.</p>
            <table>
                <tr><td><strong>Play:</strong></td><td>{play.Name}</td></tr>
                <tr><td><strong>Theatre:</strong></td><td>{theatre.Name}</td></tr>
                <tr><td><strong>Date / Time:</strong></td><td>{perf.DateTime:f}</td></tr>
                <tr><td><strong>Status:</strong></td><td>Active</td></tr>
            </table>
            <p>Enjoy the show!</p>
            """;

        var ics = BuildIcsContent(play.Name, theatre.Name, perf.DateTime, "CONFIRMED");
        var attachments = BuildCalendarAttachments(ics);
        await _emailService.SendEmailAsync(email, subject, html, attachments);
    }

    private async Task SendCancellationAsync(Reservation reservation, bool isTimelyCancellation)
    {
        var perf = reservation.Performance;
        var play = perf.Play;
        var theatre = perf.Theatre;
        var email = reservation.User.Email;

        if (string.IsNullOrEmpty(email)) return;

        var statusText = isTimelyCancellation ? "Cancelled" : "Offered (late cancellation — counts toward season limit until someone takes your spot)";
        var subject = $"Reservation {(isTimelyCancellation ? "Cancelled" : "Offered")}: {play.Name}";
        var html = $"""
            <h2>Reservation {(isTimelyCancellation ? "Cancelled" : "Offered")}</h2>
            <p>Your reservation has been {(isTimelyCancellation ? "cancelled" : "offered to other members")}.</p>
            <table>
                <tr><td><strong>Play:</strong></td><td>{play.Name}</td></tr>
                <tr><td><strong>Theatre:</strong></td><td>{theatre.Name}</td></tr>
                <tr><td><strong>Date / Time:</strong></td><td>{perf.DateTime:f}</td></tr>
                <tr><td><strong>Status:</strong></td><td>{statusText}</td></tr>
            </table>
            """;

        var ics = BuildIcsContent(play.Name, theatre.Name, perf.DateTime, "CANCELLED");
        var attachments = BuildCalendarAttachments(ics);
        await _emailService.SendEmailAsync(email, subject, html, attachments);
    }

    private static string BuildIcsContent(string summary, string location, DateTime dateTime, string status)
    {
        var dtStart = dateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
        var dtEnd = dateTime.ToUniversalTime().AddHours(2).ToString("yyyyMMdd'T'HHmmss'Z'");
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
        var uid = Guid.NewGuid().ToString();

        return $"""
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//KMD//Reservation//EN
            METHOD:{(status == "CANCELLED" ? "CANCEL" : "PUBLISH")}
            BEGIN:VEVENT
            UID:{uid}
            DTSTART:{dtStart}
            DTEND:{dtEnd}
            DTSTAMP:{stamp}
            SUMMARY:{summary}
            LOCATION:{location}
            STATUS:{status}
            END:VEVENT
            END:VCALENDAR
            """;
    }

    private static List<EmailAttachment> BuildCalendarAttachments(string icsContent)
    {
        var bytes = BinaryData.FromString(icsContent);
        return
        [
            new EmailAttachment("event.ics", "text/calendar", bytes),
            new EmailAttachment("event.ical", "text/calendar", bytes)
        ];
    }
}
