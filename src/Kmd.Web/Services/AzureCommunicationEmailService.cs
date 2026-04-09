using Azure;
using Azure.Communication.Email;

namespace Kmd.Web.Services;

public class AzureCommunicationEmailService : IEmailService
{
    private readonly EmailClient _client;
    private readonly string _senderAddress;
    private readonly ILogger<AzureCommunicationEmailService> _logger;

    public AzureCommunicationEmailService(IConfiguration configuration, ILogger<AzureCommunicationEmailService> logger)
    {
        var connectionString = configuration["Email:AzureCommunicationConnectionString"]
            ?? throw new InvalidOperationException("Email:AzureCommunicationConnectionString is not configured.");
        _senderAddress = configuration["Email:SenderAddress"]
            ?? throw new InvalidOperationException("Email:SenderAddress is not configured.");
        _client = new EmailClient(connectionString);
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, IReadOnlyList<EmailAttachment>? attachments = null)
    {
        try
        {
            var message = new EmailMessage(_senderAddress, to, new EmailContent(subject) { Html = htmlBody });

            if (attachments != null)
            {
                foreach (var a in attachments)
                {
                    message.Attachments.Add(new Azure.Communication.Email.EmailAttachment(a.FileName, a.ContentType, a.Content));
                }
            }

            var operation = await _client.SendAsync(WaitUntil.Started, message);
            _logger.LogInformation("Email sent to {To}, subject: {Subject}, operation: {OperationId}", to, subject, operation.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}, subject: {Subject}", to, subject);
            return false;
        }
    }
}
