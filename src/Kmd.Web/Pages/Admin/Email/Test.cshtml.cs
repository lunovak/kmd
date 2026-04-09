using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Email;

[Authorize(Roles = "Admin")]
public class TestModel : PageModel
{
    private readonly IEmailService _emailService;

    public TestModel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string recipientEmail)
    {
        var success = await _emailService.SendEmailAsync(
            recipientEmail,
            "KMD Email Test",
            "<h1>Test Email</h1><p>This is a test email from the KMD system to verify email configuration.</p>");

        if (success)
            TempData["Success"] = $"Test email sent successfully to {recipientEmail}.";
        else
            TempData["Error"] = "Failed to send test email. Check the application logs for details.";

        return RedirectToPage();
    }
}
