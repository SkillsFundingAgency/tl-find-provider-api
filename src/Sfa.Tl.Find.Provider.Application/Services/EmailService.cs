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
        IDictionary<string, string> tokens)
    {
        var dynamicTokens =
            tokens?.Select(x => new {key = x.Key, val = (dynamic) x.Value})
                .ToDictionary(item => item.key, item => item.val);

        return await SendEmail(recipient, templateName, dynamicTokens);
    }

    public async Task<bool> SendEmail(
        string recipient, 
        string templateName,
        Dictionary<string, dynamic> tokens = null)
    {
        var emailTemplate = await _emailTemplateRepository.GetEmailTemplate(templateName);
        if (emailTemplate == null)
        {
            _logger.LogWarning("Email template {templateName} not found. No emails sent.", templateName);
            return false;
        }

        try
        {
            var emailResponse = await _notificationClient
                .SendEmailAsync(
                    recipient, 
                    emailTemplate.TemplateId.ToString(), 
                    tokens);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Email sent - notification id '{id}', " +
                                    "reference '{reference}, " +
                                    "content '{content}'",
                    emailResponse.id, emailResponse.reference, emailResponse.content);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email template {emailTemplateId} to {recipient}.",
                emailTemplate.TemplateId, recipient);
        }

        return false;
    }

    /*
public async Task<bool> SendEmployerContactEmail(
            string fullName,
            string organisationName,
            string phone,
            string email)
        {
            var toAddresses = _configuration.SupportEmailInboxAddress?.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (toAddresses == null || !toAddresses.Any())
            {
                _logger.LogError("There are no support email addresses defined.");
                return false;
            }
            
            var tokens = new Dictionary<string, dynamic>
            {
                { "full_name", fullName },
                { "organisation_name", organisationName },
                { "organisation_phone_number", phone },
                { "organisation_email_address", email }
            };

            var allEmailsSent = true;
            foreach (var toAddress in toAddresses)
            {
                allEmailsSent &= await SendEmail(toAddress,
                    _configuration.EmployerContactEmailTemplateId,
                    tokens);
            }

            return allEmailsSent;
        }
        
        private async Task<bool> SendEmail(string recipient, string emailTemplateId,
            Dictionary<string, dynamic> personalisationTokens)
        {
            var emailSent = false;

            try
            {
                var emailResponse = await _notificationClient.SendEmailAsync(recipient, emailTemplateId, personalisationTokens);

                _logger.LogInformation($"Email sent - notification id '{emailResponse.id}', " +
                                       $"reference '{emailResponse.reference}, " +
                                       $"content '{emailResponse.content}'");
                emailSent = true;
            }
            catch (Exception ex)
            {
                var message = $"Error sending email template {emailTemplateId} to {recipient}. {ex.Message}";
                _logger.LogError(ex, message);
            }

            return emailSent;
        }     */
}
