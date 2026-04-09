using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmd.Web.Pages.Admin.Email;

[Authorize(Roles = "Admin")]
public class ComposeModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<ComposeModel> _logger;

    public ComposeModel(ApplicationDbContext db, IEmailService emailService, ILogger<ComposeModel> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    [BindProperty] public string RecipientType { get; set; } = "Season";
    [BindProperty] public int? SeasonId { get; set; }
    [BindProperty] public int? PerformanceId { get; set; }
    [BindProperty] public string Subject { get; set; } = string.Empty;
    [BindProperty] public string Body { get; set; } = string.Empty;

    public int? RecipientCount { get; set; }
    public List<Season> Seasons { get; set; } = [];
    public List<Performance> Performances { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
    }

    public async Task<IActionResult> OnPostPreviewAsync()
    {
        await LoadDropdownsAsync();
        var recipients = await GetRecipientsAsync();
        RecipientCount = recipients.Count;
        return Page();
    }

    public async Task<IActionResult> OnPostSendAsync()
    {
        await LoadDropdownsAsync();
        var recipients = await GetRecipientsAsync();
        RecipientCount = recipients.Count;

        if (recipients.Count == 0)
        {
            TempData["Error"] = "No recipients found for the selected criteria.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Body))
        {
            TempData["Error"] = "Subject and body are required.";
            return Page();
        }

        // Capture values for background task
        var emailList = recipients.Select(r => r.Email!).ToList();
        var subject = Subject;
        var body = Body;
        var adminEmail = User.Identity?.Name ?? "unknown";

        _logger.LogInformation(
            "Admin {Admin} sending bulk email: Subject='{Subject}', RecipientType={Type}, Count={Count}",
            adminEmail, subject, RecipientType, emailList.Count);

        // Send in background
        _ = Task.Run(async () =>
        {
            var success = 0;
            var failure = 0;

            foreach (var email in emailList)
            {
                try
                {
                    var sent = await _emailService.SendEmailAsync(email, subject, body);
                    if (sent) success++; else failure++;
                }
                catch
                {
                    failure++;
                }
            }

            _logger.LogInformation(
                "Bulk email completed: Subject='{Subject}', Success={Success}, Failure={Failure}",
                subject, success, failure);
        });

        TempData["Success"] = $"Bulk email queued for {emailList.Count} recipient(s). Check logs for delivery status.";
        return RedirectToPage();
    }

    private async Task<List<ApplicationUser>> GetRecipientsAsync()
    {
        if (RecipientType == "Season" && SeasonId.HasValue)
        {
            return await _db.Subscriptions
                .Where(s => s.SeasonId == SeasonId.Value)
                .Select(s => s.User)
                .Where(u => !u.LockoutEnd.HasValue && u.Email != null)
                .Distinct()
                .ToListAsync();
        }

        if (RecipientType == "Performance" && PerformanceId.HasValue)
        {
            return await _db.Reservations
                .Where(r => r.PerformanceId == PerformanceId.Value && r.Status == ReservationStatus.Active)
                .Select(r => r.User)
                .Where(u => !u.LockoutEnd.HasValue && u.Email != null)
                .Distinct()
                .ToListAsync();
        }

        return [];
    }

    private async Task LoadDropdownsAsync()
    {
        Seasons = await _db.Seasons.OrderByDescending(s => s.Id).ToListAsync();
        Performances = await _db.Performances
            .Include(p => p.Play)
            .Include(p => p.Theatre)
            .OrderByDescending(p => p.DateTime)
            .ToListAsync();
    }
}
