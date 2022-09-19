using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Models;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class EmailDeliveryStatusService : IEmailDeliveryStatusService
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailDeliveryStatusService> _logger;

    public EmailDeliveryStatusService(
        IEmailService emailService,
        IOptions<EmailSettings> emailOptions,
        ILogger<EmailDeliveryStatusService> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    public async Task<int> HandleEmailDeliveryStatus(EmailDeliveryReceipt deliveryReceipt)
    {
        /*
        var tokens = new Dictionary<string, string>()
        {
            { "summary", summary },
            { "email_type", emailTemplateName.Humanize().ToLower() },
            { "body", emailBody },
            { "reason", emailBodyDto.EmailDeliveryStatusType.Humanize() },
            { "sender_username", emailHistoryDto.CreatedBy },
            { "email_body", emailBodyDto.Body }
        };

        await _emailService.SendEmail(
            _emailSettings.SupportEmailAddress,
            EmailTemplateNames.EmailDeliveryStatus
            );
    */

        return 1;
    }
}
