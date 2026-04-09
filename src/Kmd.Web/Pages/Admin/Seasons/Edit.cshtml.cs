using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Seasons;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly SeasonService _seasonService;

    public EditModel(SeasonService seasonService)
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
        if (!ModelState.IsValid)
            return Page();

        await _seasonService.UpdateAsync(Season);
        TempData["Success"] = "Season updated successfully.";
        return RedirectToPage("Index");
    }
}
