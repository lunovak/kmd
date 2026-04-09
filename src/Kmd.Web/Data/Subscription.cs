namespace Kmd.Web.Data;

public class Subscription
{
    public string UserId { get; set; } = string.Empty;
    public int SeasonId { get; set; }
    public int ReservationsLimit { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Season Season { get; set; } = null!;
}
