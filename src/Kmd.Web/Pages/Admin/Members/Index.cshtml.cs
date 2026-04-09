using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Members;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public List<ApplicationUser> Members { get; set; } = [];

    public bool IsActive(ApplicationUser user) => _userService.IsActive(user);

    public async Task OnGetAsync()
    {
        Members = await _userService.GetAllMembersAsync();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(string id)
    {
        var (success, errors) = await _userService.SetActiveStatusAsync(id, false);
        TempData[success ? "Success" : "Error"] = success ? "Member deactivated." : string.Join(" ", errors);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReactivateAsync(string id)
    {
        var (success, errors) = await _userService.SetActiveStatusAsync(id, true);
        TempData[success ? "Success" : "Error"] = success ? "Member reactivated." : string.Join(" ", errors);
        return RedirectToPage();
    }
}
