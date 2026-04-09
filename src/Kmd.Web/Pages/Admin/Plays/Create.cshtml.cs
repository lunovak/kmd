using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kmd.Web.Pages.Admin.Plays;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly PlayService _playService;
    private readonly TheatreService _theatreService;

    public CreateModel(PlayService playService, TheatreService theatreService)
    {
        _playService = playService;
        _theatreService = theatreService;
    }

    [BindProperty]
    public Play Play { get; set; } = new();

    public List<SelectListItem> TheatreOptions { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadTheatresAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTheatresAsync();
            return Page();
        }

        await _playService.CreateAsync(Play);
        TempData["Success"] = "Play created successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadTheatresAsync()
    {
        var theatres = await _theatreService.GetAllAsync();
        TheatreOptions = theatres.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList();
    }
}
