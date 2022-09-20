using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class EmailService : IEmailService
{
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly IAsyncNotificationClient _notificationClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IEmailTemplateRepository emailTemplateRepository,
        IAsyncNotificationClient notificationClient,
        ILogger<EmailService> logger)
    {
        _emailTemplateRepository = emailTemplateRepository ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SendEmail(
        string recipient,
        string templateName,
        IDictionary<string, string> tokens = null)
    {
        var dynamicTokens =
            tokens?.Select(x => new { key = x.Key, val = (dynamic)x.Value })
                .ToDictionary(item => item.key, item => item.val);

        return await SendEmail(recipient, templateName, dynamicTokens);
    }

    public async Task<bool> SendEmail(
        string recipients,
        string templateName,
        Dictionary<string, dynamic> tokens)
    {
        var recipientList = recipients?.Split(';', StringSplitOptions.RemoveEmptyEntries);

        if (recipientList == null || !recipientList.Any())
        {
            _logger.LogWarning("There are no email recipients.");
            return false;
        }

        var emailTemplate = await _emailTemplateRepository.GetEmailTemplateByName(templateName);
        if (emailTemplate == null)
        {
            _logger.LogWarning("Email template {templateName} not found. No emails sent.", templateName);
            return false;
        }

        var allEmailsSent = true;
        foreach (var recipient in recipientList)
        {
            try
            {
                var emailResponse = await _notificationClient
                    .SendEmailAsync(
                        recipient,
                        emailTemplate.TemplateId,
                        tokens);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Email sent - notification id '{id}', " +
                                        "reference '{reference}, " +
                                        "content '{content}'",
                        emailResponse.id, emailResponse.reference, emailResponse.content);
                }
            }
            catch (Exception ex)
            {
                allEmailsSent = false;

                _logger.LogError(ex, "Error sending email template {emailTemplateId} to {recipient}.",
                emailTemplate.TemplateId, recipient);
            }
        }

        return allEmailsSent;
    }
}
