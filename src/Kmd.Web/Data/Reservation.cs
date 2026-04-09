namespace Kmd.Web.Data;

public class Reservation
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PerformanceId { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public DateTime? ReminderSentDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Performance Performance { get; set; } = null!;
}
