namespace Kmd.Web.Services;

public class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendEmailAsync(string to, string subject, string htmlBody, IReadOnlyList<EmailAttachment>? attachments = null)
    {
        _logger.LogInformation(
            "[DEV EMAIL] To: {To}, Subject: {Subject}, Attachments: {Count}\n{Body}",
            to, subject, attachments?.Count ?? 0, htmlBody);
        return Task.FromResult(true);
    }
}
