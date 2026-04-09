namespace Kmd.Web.Data;

public class Theatre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Notes { get; set; }

    public ICollection<Play> Plays { get; set; } = [];
}
