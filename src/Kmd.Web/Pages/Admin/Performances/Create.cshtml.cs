using System.ComponentModel.DataAnnotations;
using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kmd.Web.Pages.Admin.Performances;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly PerformanceService _performanceService;
    private readonly PlayService _playService;
    private readonly SeasonService _seasonService;
    private readonly ReservationWaveService _waveService;

    public CreateModel(PerformanceService performanceService, PlayService playService,
        SeasonService seasonService, ReservationWaveService waveService)
    {
        _performanceService = performanceService;
        _playService = playService;
        _seasonService = seasonService;
        _waveService = waveService;
    }

    [BindProperty, Required]
    public int PlayId { get; set; }

    [BindProperty]
    public int? SeasonId { get; set; }

    [BindProperty, Required]
    public int ReservationWaveId { get; set; }

    [BindProperty, Required]
    public DateTime DateTime { get; set; }

    [BindProperty, Required, Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    public List<SelectListItem> PlayOptions { get; set; } = [];
    public List<SelectListItem> SeasonOptions { get; set; } = [];
    public List<SelectListItem> WaveOptions { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        var play = await _playService.GetByIdAsync(PlayId);
        if (play == null)
        {
            ModelState.AddModelError(string.Empty, "Selected play not found.");
            await LoadDropdownsAsync();
            return Page();
        }

        var performance = new Performance
        {
            PlayId = PlayId,
            TheatreId = play.TheatreId,
            ReservationWaveId = ReservationWaveId,
            DateTime = DateTime,
            Capacity = Capacity
        };

        await _performanceService.CreateAsync(performance);
        TempData["Success"] = "Performance created successfully.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnGetWavesAsync(int seasonId)
    {
        var waves = await _waveService.GetBySeasonAsync(seasonId);
        return new JsonResult(waves.Select(w => new { w.Id, w.Name }));
    }

    private async Task LoadDropdownsAsync()
    {
        var plays = await _playService.GetAllAsync();
        PlayOptions = plays.Select(p => new SelectListItem($"{p.Name} ({p.Theatre.Name})", p.Id.ToString())).ToList();

        var seasons = await _seasonService.GetAllAsync();
        SeasonOptions = seasons.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();

        if (SeasonId.HasValue)
        {
            var waves = await _waveService.GetBySeasonAsync(SeasonId.Value);
            WaveOptions = waves.Select(w => new SelectListItem(w.Name, w.Id.ToString())).ToList();
        }
    }
}
