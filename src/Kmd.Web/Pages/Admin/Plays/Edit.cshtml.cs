using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kmd.Web.Pages.Admin.Plays;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly PlayService _playService;
    private readonly TheatreService _theatreService;

    public EditModel(PlayService playService, TheatreService theatreService)
    {
        _playService = playService;
        _theatreService = theatreService;
    }

    [BindProperty]
    public Play Play { get; set; } = new();

    public List<SelectListItem> TheatreOptions { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var play = await _playService.GetByIdAsync(id);
        if (play == null) return NotFound();

        Play = play;
        await LoadTheatresAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTheatresAsync();
            return Page();
        }

        await _playService.UpdateAsync(Play);
        TempData["Success"] = "Play updated successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadTheatresAsync()
    {
        var theatres = await _theatreService.GetAllAsync();
        TheatreOptions = theatres.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
    }
}
