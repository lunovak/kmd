using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kmd.Web.Pages.Admin.Waves;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ReservationWaveService _waveService;
    private readonly SeasonService _seasonService;

    public CreateModel(ReservationWaveService waveService, SeasonService seasonService)
    {
        _waveService = waveService;
        _seasonService = seasonService;
    }

    [BindProperty]
    public ReservationWave Wave { get; set; } = new();

    public List<SelectListItem> SeasonOptions { get; set; } = [];

    public async Task OnGetAsync(int? seasonId)
    {
        if (seasonId.HasValue)
            Wave.SeasonId = seasonId.Value;
        await LoadSeasonsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSeasonsAsync();
            return Page();
        }

        await _waveService.CreateAsync(Wave);
        TempData["Success"] = "Reservation wave created successfully.";
        return RedirectToPage("Index", new { seasonId = Wave.SeasonId });
    }

    private async Task LoadSeasonsAsync()
    {
        var seasons = await _seasonService.GetAllAsync();
        SeasonOptions = seasons.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
    }
}
