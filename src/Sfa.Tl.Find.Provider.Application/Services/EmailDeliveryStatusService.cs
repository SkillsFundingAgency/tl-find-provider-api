using Humanizer;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Models;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Application.Services;

public class EmailDeliveryStatusService : IEmailDeliveryStatusService
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailDeliveryStatusService> _logger;

    public EmailDeliveryStatusService(
        IEmailService emailService,
        IEmailTemplateRepository emailTemplateRepository,
        IOptions<EmailSettings> emailOptions,
        ILogger<EmailDeliveryStatusService> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _emailTemplateRepository = emailTemplateRepository ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    public async Task<int> HandleEmailDeliveryStatus(EmailDeliveryReceipt deliveryReceipt)
    {
        if (string.Compare(deliveryReceipt.EmailDeliveryStatus, EmailDeliveryStatus.Delivered, StringComparison.OrdinalIgnoreCase) == 0)
        {
            return 0;
        }

        var emailTemplate = await _emailTemplateRepository
            .GetEmailTemplate(deliveryReceipt.TemplateId.ToString());
        var emailTemplateName = emailTemplate != null 
            ? emailTemplate.Name 
            : $"Unknown template {deliveryReceipt.TemplateId}";

        var tokens = new Dictionary<string, string>()
        {
            { "email_type", emailTemplateName },//.Humanize().ToLower() },
            { "reference", deliveryReceipt.Reference },
            { "reason", deliveryReceipt.EmailDeliveryStatus.Humanize() },
            { "sender_username", deliveryReceipt.To },
        };
        
        await _emailService.SendEmail(
            _emailSettings.SupportEmailAddress,
            EmailTemplateNames.EmailDeliveryStatus,
            tokens
            );
    

        return 1;
    }
}
