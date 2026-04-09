using System.ComponentModel.DataAnnotations;
using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Theatres;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly TheatreService _theatreService;

    public CreateModel(TheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [BindProperty]
    public Theatre Theatre { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _theatreService.CreateAsync(Theatre);
        TempData["Success"] = "Theatre created successfully.";
        return RedirectToPage("Index");
    }
}
