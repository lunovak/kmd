using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Kmd.Web.Pages.Performances;

public class IndexModel : PageModel
{
    private readonly PerformanceService _performanceService;
    private readonly SubscriptionService _subscriptionService;
    private readonly SeasonService _seasonService;
    private readonly ReservationService _reservationService;

    public IndexModel(PerformanceService performanceService, SubscriptionService subscriptionService,
        SeasonService seasonService, ReservationService reservationService)
    {
        _performanceService = performanceService;
        _subscriptionService = subscriptionService;
        _seasonService = seasonService;
        _reservationService = reservationService;
    }

    public List<WaveGroup> WaveGroups { get; set; } = [];
    public Subscription? Subscription { get; set; }
    public int MemberActiveReservationCount { get; set; }
    public string? CurrentSeasonName { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostReserveAsync(int performanceId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _reservationService.CreateReservationAsync(userId, performanceId);

        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToPage();
    }

    private async Task LoadDataAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var now = DateTime.UtcNow;

        var performances = await _performanceService.GetOpenPerformancesAsync();

        var memberReservations = await _reservationService.GetUserReservationMapAsync(userId);

        var grouped = performances
            .GroupBy(p => p.ReservationWave)
            .OrderBy(g => g.Key.StartDate)
            .Select(g => new WaveGroup
            {
                Wave = g.Key,
                Performances = g.Select(p =>
                {
                    var activeCount = p.Reservations.Count(r => r.Status == ReservationStatus.Active);
                    memberReservations.TryGetValue(p.Id, out var memberStatus);
                    return new PerformanceItem
                    {
                        Performance = p,
                        ActiveCount = activeCount,
                        MemberReservationStatus = memberStatus
                    };
                }).ToList()
            }).ToList();

        WaveGroups = grouped;

        // Load subscription info for current season
        var latestSeasonId = await _seasonService.GetLatestSeasonIdAsync();
        if (latestSeasonId.HasValue)
        {
            Subscription = await _subscriptionService.GetAsync(userId, latestSeasonId.Value);
            MemberActiveReservationCount = await _reservationService.GetSeasonReservationCountAsync(userId, latestSeasonId.Value);
            var season = await _seasonService.GetByIdAsync(latestSeasonId.Value);
            CurrentSeasonName = season?.Name;
        }
    }
}

public class WaveGroup
{
    public ReservationWave Wave { get; set; } = null!;
    public List<PerformanceItem> Performances { get; set; } = [];
}

public class PerformanceItem
{
    public Performance Performance { get; set; } = null!;
    public int ActiveCount { get; set; }
    public ReservationStatus? MemberReservationStatus { get; set; }
}
