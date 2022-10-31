using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
public  class EmailDeliveryStatusServiceBuilder
{
    public IEmailDeliveryStatusService Build(
        IEmailService emailService = null,
        IEmailTemplateRepository emailTemplateRepository = null,
        EmailSettings emailSettings = null,
        ILogger<EmailDeliveryStatusService> logger = null)
    {
        emailService ??= Substitute.For<IEmailService>();
        emailTemplateRepository ??= Substitute.For<IEmailTemplateRepository>();
        logger ??= Substitute.For<ILogger<EmailDeliveryStatusService>>();

        emailSettings ??= new SettingsBuilder().BuildEmailSettings();
        var emailOptions = emailSettings.ToOptions();
        
        return new EmailDeliveryStatusService(
            emailService,
            emailTemplateRepository,
            emailOptions,
            logger);
    }
}
