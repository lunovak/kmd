using System.ComponentModel.DataAnnotations;
using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Reservations;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;
    private readonly PerformanceService _performanceService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ReservationService reservationService, UserService userService,
        PerformanceService performanceService, ILogger<CreateModel> logger)
    {
        _reservationService = reservationService;
        _userService = userService;
        _performanceService = performanceService;
        _logger = logger;
    }

    [BindProperty, Required]
    public string UserId { get; set; } = string.Empty;

    [BindProperty, Required]
    public int PerformanceId { get; set; }

    public List<ApplicationUser> Members { get; set; } = [];
    public List<Performance> PerformanceOptions { get; set; } = [];

    public async Task OnGetAsync(string? userId, int? performanceId)
    {
        UserId = userId ?? string.Empty;
        PerformanceId = performanceId ?? 0;
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var adminEmail = User.Identity?.Name;
        _logger.LogInformation("Admin {Admin} creating reservation for user {UserId} performance {PerformanceId}",
            adminEmail, UserId, PerformanceId);

        var result = await _reservationService.CreateReservationAsync(UserId, PerformanceId, adminOverride: true);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            await LoadOptionsAsync();
            return Page();
        }

        _logger.LogInformation("Admin {Admin} successfully created reservation for user {UserId} performance {PerformanceId}",
            adminEmail, UserId, PerformanceId);

        TempData["Success"] = $"Reservation created for member.";
        return RedirectToPage("BySeason");
    }

    private async Task LoadOptionsAsync()
    {
        Members = await _userService.GetAllMembersAsync();
        PerformanceOptions = await _performanceService.GetAllAsync();
    }
}
