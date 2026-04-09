using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Theatres;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly TheatreService _theatreService;

    public EditModel(TheatreService theatreService)
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
        if (!ModelState.IsValid)
            return Page();

        await _theatreService.UpdateAsync(Theatre);
        TempData["Success"] = "Theatre updated successfully.";
        return RedirectToPage("Index");
    }
}
