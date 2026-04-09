using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Plays;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly PlayService _playService;

    public DeleteModel(PlayService playService)
    {
        _playService = playService;
    }

    [BindProperty]
    public Play Play { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var play = await _playService.GetByIdAsync(id);
        if (play == null) return NotFound();

        Play = play;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await _playService.CanDeleteAsync(Play.Id))
        {
            TempData["Error"] = "Cannot delete this play because it has associated performances.";
            return RedirectToPage("Index");
        }

        await _playService.DeleteAsync(Play.Id);
        TempData["Success"] = "Play deleted successfully.";
        return RedirectToPage("Index");
    }
}
