using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Performances;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly PerformanceService _performanceService;

    public DeleteModel(PerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    [BindProperty]
    public Performance Performance { get; set; } = new();

    public int ActiveReservationCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var perf = await _performanceService.GetByIdAsync(id);
        if (perf == null) return NotFound();

        Performance = perf;
        ActiveReservationCount = await _performanceService.GetActiveReservationCountAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var count = await _performanceService.GetActiveReservationCountAsync(Performance.Id);
        if (count > 0)
        {
            TempData["Error"] = $"Cannot delete this performance because it has {count} active reservation(s).";
            return RedirectToPage("Index");
        }

        await _performanceService.DeleteAsync(Performance.Id);
        TempData["Success"] = "Performance deleted successfully.";
        return RedirectToPage("Index");
    }
}
