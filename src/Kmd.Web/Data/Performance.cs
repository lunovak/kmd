namespace Kmd.Web.Data;

public class Performance
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int Capacity { get; set; }
    public int ReservationWaveId { get; set; }
    public int PlayId { get; set; }
    public int TheatreId { get; set; }

    public ReservationWave ReservationWave { get; set; } = null!;
    public Play Play { get; set; } = null!;
    public Theatre Theatre { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = [];
}
