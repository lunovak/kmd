namespace Kmd.Web.Data;

public class Play
{
    public int Id { get; set; }
    public int TheatreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? WikiUrl { get; set; }

    public Theatre Theatre { get; set; } = null!;
    public ICollection<Performance> Performances { get; set; } = [];
}
