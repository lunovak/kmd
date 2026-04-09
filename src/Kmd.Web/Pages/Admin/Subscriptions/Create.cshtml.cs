using System.ComponentModel.DataAnnotations;
using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Subscriptions;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly SubscriptionService _subscriptionService;

    public CreateModel(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [BindProperty]
    public int SeasonId { get; set; }

    [BindProperty, Range(1, int.MaxValue, ErrorMessage = "Limit must be a positive integer.")]
    public int DefaultLimit { get; set; } = 5;

    [BindProperty]
    public List<string> SelectedUserIds { get; set; } = [];

    public List<ApplicationUser> AvailableMembers { get; set; } = [];

    public async Task OnGetAsync(int seasonId)
    {
        SeasonId = seasonId;
        AvailableMembers = await _subscriptionService.GetUnsubscribedMembersAsync(seasonId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || SelectedUserIds.Count == 0)
        {
            if (SelectedUserIds.Count == 0)
                ModelState.AddModelError(string.Empty, "Please select at least one member.");
            AvailableMembers = await _subscriptionService.GetUnsubscribedMembersAsync(SeasonId);
            return Page();
        }

        await _subscriptionService.BulkCreateAsync(SelectedUserIds, SeasonId, DefaultLimit);
        TempData["Success"] = $"{SelectedUserIds.Count} member(s) added to season.";
        return RedirectToPage("Index", new { seasonId = SeasonId });
    }
}
