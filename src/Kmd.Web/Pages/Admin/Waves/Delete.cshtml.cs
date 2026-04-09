using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Waves;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ReservationWaveService _waveService;

    public DeleteModel(ReservationWaveService waveService)
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

    public async Task<IActionResult> OnPostAsync(int seasonId)
    {
        if (!await _waveService.CanDeleteAsync(Wave.Id))
        {
            TempData["Error"] = "Cannot delete this wave because it has associated performances.";
            return RedirectToPage("Index", new { seasonId });
        }

        await _waveService.DeleteAsync(Wave.Id);
        TempData["Success"] = "Reservation wave deleted successfully.";
        return RedirectToPage("Index", new { seasonId });
    }
}
