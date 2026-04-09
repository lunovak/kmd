using System.ComponentModel.DataAnnotations;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Members;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly UserService _userService;

    public EditModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public string Id { get; set; } = string.Empty;

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string? ClassName { get; set; }

    [BindProperty]
    public string? OtherContact { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        Id = user.Id;
        Email = user.Email!;
        ClassName = user.ClassName;
        OtherContact = user.OtherContact;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var (success, errors) = await _userService.UpdateMemberAsync(Id, Email, ClassName, OtherContact);
        if (!success)
        {
            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);
            return Page();
        }

        TempData["Success"] = "Member updated successfully.";
        return RedirectToPage("Index");
    }

    public async Task<IActionResult> OnPostResetPasswordAsync(string id, string newPassword)
    {
        var (success, errors) = await _userService.ResetPasswordAsync(id, newPassword);
        TempData[success ? "Success" : "Error"] = success ? "Password reset successfully." : string.Join(" ", errors);
        return RedirectToPage("Edit", new { id });
    }
}
