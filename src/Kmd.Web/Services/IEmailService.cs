namespace Kmd.Web.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, IReadOnlyList<EmailAttachment>? attachments = null);
}

public record EmailAttachment(string FileName, string ContentType, BinaryData Content);
