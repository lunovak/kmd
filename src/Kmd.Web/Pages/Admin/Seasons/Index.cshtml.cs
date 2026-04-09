using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Seasons;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly SeasonService _seasonService;

    public IndexModel(SeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    public List<Season> Seasons { get; set; } = [];
    public int? LatestSeasonId { get; set; }

    public async Task OnGetAsync()
    {
        Seasons = await _seasonService.GetAllAsync();
        LatestSeasonId = await _seasonService.GetLatestSeasonIdAsync();
    }
}
