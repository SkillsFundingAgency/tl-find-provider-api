using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class EmailServiceBuilder
{
    public IEmailService Build(
        IEmailTemplateRepository emailTemplateRepository = null,
        IAsyncNotificationClient notificationClient = null,
        ILogger<EmailService> logger = null)
    {
        emailTemplateRepository ??= Substitute.For<IEmailTemplateRepository>();
        notificationClient ??= Substitute.For<IAsyncNotificationClient>();
        logger ??= Substitute.For<ILogger<EmailService>>();

        return new EmailService(
            emailTemplateRepository,
            notificationClient,
            logger);
    }
}