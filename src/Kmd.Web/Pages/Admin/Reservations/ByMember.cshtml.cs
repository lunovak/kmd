using Kmd.Web.Data;
using Kmd.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmd.Web.Pages.Admin.Reservations;

[Authorize(Roles = "Admin")]
public class ByMemberModel : PageModel
{
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;

    public ByMemberModel(ReservationService reservationService, UserService userService)
    {
        _reservationService = reservationService;
        _userService = userService;
    }

    public List<Reservation> Reservations { get; set; } = [];
    public List<ApplicationUser> Members { get; set; } = [];
    public string? MemberId { get; set; }

    public async Task OnGetAsync(string? userId)
    {
        MemberId = userId;
        Members = await _userService.GetAllMembersAsync();

        if (!string.IsNullOrEmpty(userId))
            Reservations = await _reservationService.GetByMemberAsync(userId);
    }
}
