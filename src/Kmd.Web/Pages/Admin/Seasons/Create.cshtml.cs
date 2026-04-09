using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Seasons;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly SeasonService _seasonService;

    public CreateModel(SeasonService seasonService)
    {
        _seasonService = seasonService;
    }

    [BindProperty]
    public Season Season { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _seasonService.CreateAsync(Season);
        TempData["Success"] = "Season created successfully.";
        return RedirectToPage("Index");
    }
}
