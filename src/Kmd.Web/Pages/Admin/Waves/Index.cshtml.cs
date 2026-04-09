using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Waves;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ReservationWaveService _waveService;
    private readonly SeasonService _seasonService;

    public IndexModel(ReservationWaveService waveService, SeasonService seasonService)
    {
        _waveService = waveService;
        _seasonService = seasonService;
    }

    public List<ReservationWave> Waves { get; set; } = [];
    public List<Season> Seasons { get; set; } = [];
    public int? SeasonId { get; set; }

    public async Task OnGetAsync(int? seasonId)
    {
        Seasons = await _seasonService.GetAllAsync();
        SeasonId = seasonId ?? await _seasonService.GetLatestSeasonIdAsync();

        if (SeasonId.HasValue)
            Waves = await _waveService.GetBySeasonAsync(SeasonId.Value);
    }
}
