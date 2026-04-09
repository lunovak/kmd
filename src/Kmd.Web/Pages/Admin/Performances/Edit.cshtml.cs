using System.ComponentModel.DataAnnotations;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kmd.Web.Pages.Admin.Performances;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly PerformanceService _performanceService;
    private readonly ReservationWaveService _waveService;

    public EditModel(PerformanceService performanceService, ReservationWaveService waveService)
    {
        _performanceService = performanceService;
        _waveService = waveService;
    }

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public int PlayId { get; set; }

    [BindProperty]
    public int TheatreId { get; set; }

    [BindProperty, Required]
    public int ReservationWaveId { get; set; }

    [BindProperty, Required]
    public DateTime DateTime { get; set; }

    [BindProperty, Required, Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    public string PlayName { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;
    public int ActiveReservationCount { get; set; }
    public List<SelectListItem> WaveOptions { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var perf = await _performanceService.GetByIdAsync(id);
        if (perf == null) return NotFound();

        Id = perf.Id;
        PlayId = perf.PlayId;
        TheatreId = perf.TheatreId;
        ReservationWaveId = perf.ReservationWaveId;
        DateTime = perf.DateTime;
        Capacity = perf.Capacity;
        PlayName = perf.Play.Name;
        TheatreName = perf.Theatre.Name;
        ActiveReservationCount = await _performanceService.GetActiveReservationCountAsync(id);

        await LoadWavesAsync(perf.ReservationWave.SeasonId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await ReloadDisplayAsync();
            return Page();
        }

        var activeCount = await _performanceService.GetActiveReservationCountAsync(Id);
        if (Capacity < activeCount)
        {
            ModelState.AddModelError(nameof(Capacity), $"Capacity cannot be less than the current {activeCount} active reservations.");
            await ReloadDisplayAsync();
            return Page();
        }

        var existing = await _performanceService.GetByIdAsync(Id);
        if (existing == null) return NotFound();

        existing.ReservationWaveId = ReservationWaveId;
        existing.DateTime = DateTime;
        existing.Capacity = Capacity;

        await _performanceService.UpdateAsync(existing);
        TempData["Success"] = "Performance updated successfully.";
        return RedirectToPage("Index");
    }

    private async Task LoadWavesAsync(int seasonId)
    {
        var waves = await _waveService.GetBySeasonAsync(seasonId);
        WaveOptions = waves.Select(w => new SelectListItem(w.Name, w.Id.ToString())).ToList();
    }

    private async Task ReloadDisplayAsync()
    {
        var perf = await _performanceService.GetByIdAsync(Id);
        if (perf != null)
        {
            PlayName = perf.Play.Name;
            TheatreName = perf.Theatre.Name;
            ActiveReservationCount = await _performanceService.GetActiveReservationCountAsync(Id);
            await LoadWavesAsync(perf.ReservationWave.SeasonId);
        }
    }
}
