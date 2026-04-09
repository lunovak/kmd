using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Subscriptions;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly SubscriptionService _subscriptionService;
    private readonly SeasonService _seasonService;

    public IndexModel(SubscriptionService subscriptionService, SeasonService seasonService)
    {
        _subscriptionService = subscriptionService;
        _seasonService = seasonService;
    }

    public List<SubscriptionViewModel> Subscriptions { get; set; } = [];
    public List<Season> Seasons { get; set; } = [];
    public int? SeasonId { get; set; }

    public async Task OnGetAsync(int? seasonId)
    {
        Seasons = await _seasonService.GetAllAsync();
        SeasonId = seasonId ?? await _seasonService.GetLatestSeasonIdAsync();

        if (SeasonId.HasValue)
            Subscriptions = await _subscriptionService.GetBySeasonAsync(SeasonId.Value);
    }

    public async Task<IActionResult> OnPostUpdateLimitAsync(string userId, int seasonId, int newLimit)
    {
        if (newLimit < 1)
        {
            TempData["Error"] = "Limit must be a positive integer.";
            return RedirectToPage(new { seasonId });
        }

        await _subscriptionService.UpdateLimitAsync(userId, seasonId, newLimit);
        TempData["Success"] = "Limit updated.";
        return RedirectToPage(new { seasonId });
    }

    public async Task<IActionResult> OnPostRemoveAsync(string userId, int seasonId)
    {
        if (!await _subscriptionService.CanRemoveAsync(userId, seasonId))
        {
            TempData["Error"] = "Cannot remove subscription: member has active reservations in this season.";
            return RedirectToPage(new { seasonId });
        }

        await _subscriptionService.RemoveAsync(userId, seasonId);
        TempData["Success"] = "Subscription removed.";
        return RedirectToPage(new { seasonId });
    }
}
