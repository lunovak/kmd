using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Performances;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly PerformanceService _performanceService;
    private readonly SeasonService _seasonService;
    private readonly ReservationWaveService _waveService;
    private readonly TheatreService _theatreService;

    public IndexModel(PerformanceService performanceService, SeasonService seasonService,
        ReservationWaveService waveService, TheatreService theatreService)
    {
        _performanceService = performanceService;
        _seasonService = seasonService;
        _waveService = waveService;
        _theatreService = theatreService;
    }

    public List<Performance> Performances { get; set; } = [];
    public List<Season> Seasons { get; set; } = [];
    public List<ReservationWave> Waves { get; set; } = [];
    public List<Theatre> Theatres { get; set; } = [];
    public int? SeasonFilter { get; set; }
    public int? WaveFilter { get; set; }
    public int? TheatreFilter { get; set; }

    public async Task OnGetAsync(int? seasonId, int? waveId, int? theatreId)
    {
        SeasonFilter = seasonId;
        WaveFilter = waveId;
        TheatreFilter = theatreId;

        Seasons = await _seasonService.GetAllAsync();
        Theatres = await _theatreService.GetAllAsync();

        if (seasonId.HasValue)
            Waves = await _waveService.GetBySeasonAsync(seasonId.Value);

        Performances = await _performanceService.GetAllAsync(seasonId, waveId, theatreId);
    }
}
