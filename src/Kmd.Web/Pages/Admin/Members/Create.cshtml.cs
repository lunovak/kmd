using System.ComponentModel.DataAnnotations;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Members;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly UserService _userService;

    public CreateModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string? ClassName { get; set; }

    [BindProperty]
    public string? OtherContact { get; set; }

    [BindProperty, Required]
    public string Password { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (success, errors) = await _userService.CreateMemberAsync(Email, ClassName, OtherContact, Password);
        if (!success)
        {
            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        TempData["Success"] = "Member created successfully.";
        return RedirectToPage("Index");
    }
}
