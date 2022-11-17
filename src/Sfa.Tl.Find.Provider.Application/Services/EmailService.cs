using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class EmailService : IEmailService
{
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly IAsyncNotificationClient _notificationClient;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IEmailTemplateRepository emailTemplateRepository,
        IAsyncNotificationClient notificationClient,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<EmailService> logger)
    {
        _emailTemplateRepository = emailTemplateRepository ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> SendEmail(
        string recipient,
        string templateName,
        IDictionary<string, string> tokens = null,
        string reference = null)
    {
        var dynamicTokens =
            tokens?.Select(x => new { key = x.Key, val = (dynamic)x.Value })
                .ToDictionary(item => item.key, item => item.val);

        return await SendEmail(recipient, templateName, dynamicTokens, reference);
    }

    public async Task<bool> SendEmail(
        string recipients,
        string templateName,
        Dictionary<string, dynamic> tokens,
        string reference)
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
                var (retryPolicy, context) = _policyRegistry.GetNotifyRetryPolicy(_logger);

                var emailResponse =
                    await retryPolicy
                        .ExecuteAsync(async _ =>
                                await _notificationClient
                                    .SendEmailAsync(
                                        recipient,
                                        emailTemplate.TemplateId,
                                        tokens,
                                        clientReference: reference),
                            context);

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
