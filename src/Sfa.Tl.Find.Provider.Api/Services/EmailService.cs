using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Services;
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IAsyncNotificationClient _notificationClient;
    private readonly EmployerRegistrationSettings _settings;

    public EmailService(
        IOptions<EmployerRegistrationSettings> options,
        IAsyncNotificationClient notificationClient,
        ILogger<EmailService> logger)
    {
        _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<bool> SendEmployerInterestEmail(
        string employerName,
        string employerTelephone,
        string employerEmail,
        string providers)
    {
        var toAddresses = _settings.InboxAddress
                ?.Split(';', StringSplitOptions.RemoveEmptyEntries);

        var emailTemplateId = _settings.EmployerInterestEmailTemplateId;

        if (toAddresses == null || !toAddresses.Any())
        {
            _logger.LogError("There are no destination email addresses defined.");
            return false;
        }

        var tokens = new Dictionary<string, dynamic>
            {
                { "employer_name", employerName },
                { "employer_phone_number", employerTelephone },
                { "employer_email_address", employerEmail },
                { "providers_list", providers }
            };

        var allEmailsSent = true;
        foreach (var toAddress in toAddresses)
        {
            allEmailsSent &= await SendEmail(toAddress,
                emailTemplateId,
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
    }
}
