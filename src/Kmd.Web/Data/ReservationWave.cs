namespace Kmd.Web.Data;

public class ReservationWave
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int PriorityPhaseLengthDays { get; set; }
    public int CanCancelBeforeDays { get; set; }

    public Season Season { get; set; } = null!;
    public ICollection<Performance> Performances { get; set; } = [];
}
