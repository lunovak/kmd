using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Reservations;

[Authorize(Roles = "Admin")]
public class CancelModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly ILogger<CancelModel> _logger;

    public CancelModel(ReservationService reservationService, ILogger<CancelModel> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public bool IsTimelyCancellation { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null) return NotFound();
        if (reservation.Status != ReservationStatus.Active) return RedirectToPage("BySeason");

        Reservation = reservation;
        var wave = reservation.Performance.ReservationWave;
        var cancelDeadline = reservation.Performance.DateTime.AddDays(-wave.CanCancelBeforeDays);
        IsTimelyCancellation = DateTime.UtcNow <= cancelDeadline;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool forceCancel)
    {
        var adminEmail = User.Identity?.Name;
        _logger.LogInformation("Admin {Admin} cancelling reservation {ReservationId} (forceCancel={ForceCancel})",
            adminEmail, Reservation.Id, forceCancel);

        var result = await _reservationService.CancelReservationAsync(
            Reservation.UserId ?? string.Empty, Reservation.Id, forceCancel);

        if (!result.Success)
        {
            // If userId mismatch, retry with the actual userId from the reservation
            var reservation = await _reservationService.GetByIdAsync(Reservation.Id);
            if (reservation != null)
            {
                result = await _reservationService.CancelReservationAsync(reservation.UserId, Reservation.Id, forceCancel);
            }
        }

        _logger.LogInformation("Admin {Admin} cancel result for reservation {ReservationId}: {Success} - {Message}",
            adminEmail, Reservation.Id, result.Success, result.Message);

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToPage("BySeason");
    }
}
