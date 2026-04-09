using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Theatres;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly TheatreService _theatreService;

    public DeleteModel(TheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [BindProperty]
    public Theatre Theatre { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var theatre = await _theatreService.GetByIdAsync(id);
        if (theatre == null) return NotFound();

        Theatre = theatre;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _theatreService.CanDeleteAsync(Theatre.Id))
        {
            TempData["Error"] = "Cannot delete this theatre because it has associated plays or performances.";
            return RedirectToPage("Index");
        }

        await _theatreService.DeleteAsync(Theatre.Id);
        TempData["Success"] = "Theatre deleted successfully.";
        return RedirectToPage("Index");
    }
}
