using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Kmd.Web.Pages.MyReservations;

public class IndexModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly SubscriptionService _subscriptionService;
    private readonly SeasonService _seasonService;

    public IndexModel(ReservationService reservationService, SubscriptionService subscriptionService,
        SeasonService seasonService)
    {
        _reservationService = reservationService;
        _subscriptionService = subscriptionService;
        _seasonService = seasonService;
    }

    public List<Reservation> Reservations { get; set; } = [];
    public Subscription? Subscription { get; set; }
    public int MemberSeasonCount { get; set; }
    public string? CurrentSeasonName { get; set; }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        Reservations = await _reservationService.GetUserReservationsAsync(userId);

        var latestSeasonId = await _seasonService.GetLatestSeasonIdAsync();
        if (latestSeasonId.HasValue)
        {
            Subscription = await _subscriptionService.GetAsync(userId, latestSeasonId.Value);
            MemberSeasonCount = await _reservationService.GetSeasonReservationCountAsync(userId, latestSeasonId.Value);
            var season = await _seasonService.GetByIdAsync(latestSeasonId.Value);
            CurrentSeasonName = season?.Name;
        }
    }
}
