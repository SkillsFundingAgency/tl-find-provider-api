using Sfa.Tl.Find.Provider.Application.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
public  class EmailDeliveryStatusServiceBuilder
{
    public IEmailDeliveryStatusService Build(
        IEmailService emailService = null,
        EmailSettings emailSettings = null,
        ILogger<EmailDeliveryStatusService> logger = null)
    {
        emailService ??= Substitute.For<IEmailService>();
        logger ??= Substitute.For<ILogger<EmailDeliveryStatusService>>();

        emailSettings ??= new SettingsBuilder().BuildEmailSettings();
        var emailOptions = emailSettings.ToOptions();
        
        return new EmailDeliveryStatusService(
            emailService,
            emailOptions,
            logger);
    }
}
