using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Reservations;

[Authorize(Roles = "Admin")]
public class BySeasonModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly SeasonService _seasonService;

    public BySeasonModel(ReservationService reservationService, SeasonService seasonService)
    {
        _reservationService = reservationService;
        _seasonService = seasonService;
    }

    public List<Reservation> Reservations { get; set; } = [];
    public List<Season> Seasons { get; set; } = [];
    public int? SeasonId { get; set; }

    public async Task OnGetAsync(int? seasonId)
    {
        Seasons = await _seasonService.GetAllAsync();
        SeasonId = seasonId ?? await _seasonService.GetLatestSeasonIdAsync();

        if (SeasonId.HasValue)
            Reservations = await _reservationService.GetBySeasonAsync(SeasonId.Value);
    }
}
