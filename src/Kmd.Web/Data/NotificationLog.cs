namespace Kmd.Web.Data;

public class NotificationLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PerformanceId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Performance Performance { get; set; } = null!;
}
