namespace Kmd.Web.Data;

public class Season
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }

    public ICollection<ReservationWave> ReservationWaves { get; set; } = [];
    public ICollection<Subscription> Subscriptions { get; set; } = [];
}
