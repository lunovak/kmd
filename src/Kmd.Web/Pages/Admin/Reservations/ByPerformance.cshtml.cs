using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Reservations;

[Authorize(Roles = "Admin")]
public class ByPerformanceModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly PerformanceService _performanceService;

    public ByPerformanceModel(ReservationService reservationService, PerformanceService performanceService)
    {
        _reservationService = reservationService;
        _performanceService = performanceService;
    }

    public List<Reservation> Reservations { get; set; } = [];
    public List<Performance> PerformanceOptions { get; set; } = [];
    public int? PerformanceId { get; set; }

    public async Task OnGetAsync(int? performanceId)
    {
        PerformanceId = performanceId;
        PerformanceOptions = await _performanceService.GetAllAsync();

        if (performanceId.HasValue)
            Reservations = await _reservationService.GetByPerformanceAsync(performanceId.Value);
    }
}
