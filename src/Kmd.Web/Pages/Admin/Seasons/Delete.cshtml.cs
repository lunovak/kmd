using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Seasons;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly SeasonService _seasonService;

    public DeleteModel(SeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [BindProperty]
    public Season Season { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var season = await _seasonService.GetByIdAsync(id);
        if (season == null) return NotFound();

        Season = season;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _seasonService.CanDeleteAsync(Season.Id))
        {
            TempData["Error"] = "Cannot delete this season because it has associated waves, performances, or subscriptions.";
            return RedirectToPage("Index");
        }

        await _seasonService.DeleteAsync(Season.Id);
        TempData["Success"] = "Season deleted successfully.";
        return RedirectToPage("Index");
    }
}
