using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Kmd.Web.Pages.MyReservations;

public class CancelModel : PageModel
{
    private readonly ReservationService _reservationService;

    public CancelModel(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [BindProperty]
    public Reservation Reservation { get; set; } = new();

    public bool IsTimelyCancellation { get; set; }
    public DateTime CancelDeadline { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (reservation.UserId != userId) return Forbid();
        if (reservation.Status != ReservationStatus.Active) return RedirectToPage("Index");

        Reservation = reservation;
        var wave = reservation.Performance.ReservationWave;
        CancelDeadline = reservation.Performance.DateTime.AddDays(-wave.CanCancelBeforeDays);
        IsTimelyCancellation = DateTime.UtcNow <= CancelDeadline;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _reservationService.CancelReservationAsync(userId, Reservation.Id);

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToPage("Index");
    }
}
