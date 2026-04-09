using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Plays;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly PlayService _playService;
    private readonly TheatreService _theatreService;

    public IndexModel(PlayService playService, TheatreService theatreService)
    {
        _playService = playService;
        _theatreService = theatreService;
    }

    public List<Play> Plays { get; set; } = [];
    public List<Theatre> Theatres { get; set; } = [];
    public int? TheatreFilter { get; set; }

    public async Task OnGetAsync(int? theatreId)
    {
        TheatreFilter = theatreId;
        Theatres = await _theatreService.GetAllAsync();
        Plays = await _playService.GetAllAsync(theatreId);
    }
}
