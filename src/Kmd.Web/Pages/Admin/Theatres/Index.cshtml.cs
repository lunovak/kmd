using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Theatres;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly TheatreService _theatreService;

    public IndexModel(TheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    public List<Theatre> Theatres { get; set; } = [];

    public async Task OnGetAsync()
    {
        Theatres = await _theatreService.GetAllAsync();
    }
}
