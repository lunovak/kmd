using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Waves;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ReservationWaveService _waveService;

    public EditModel(ReservationWaveService waveService)
    {
        _waveService = waveService;
    }

    [BindProperty]
    public ReservationWave Wave { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var wave = await _waveService.GetByIdAsync(id);
        if (wave == null) return NotFound();

        Wave = wave;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _waveService.UpdateAsync(Wave);
        TempData["Success"] = "Reservation wave updated successfully.";
        return RedirectToPage("Index", new { seasonId = Wave.SeasonId });
    }
}
