using Microsoft.Extensions.Logging;
using Notify.Interfaces;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Policies;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class EmailServiceBuilder
{
    public IEmailService Build(
        IEmailTemplateRepository emailTemplateRepository = null,
        IAsyncNotificationClient notificationClient = null,
        IReadOnlyPolicyRegistry<string> policyRegistry = null,
        ILogger<EmailService> logger = null)
    {
        emailTemplateRepository ??= Substitute.For<IEmailTemplateRepository>();
        notificationClient ??= Substitute.For<IAsyncNotificationClient>();

        policyRegistry ??= PollyPolicyBuilder
            .BuildGovNotifyPolicyAndRegistry()
            .Registry;

        logger ??= Substitute.For<ILogger<EmailService>>();

        return new EmailService(
            emailTemplateRepository,
            notificationClient,
            policyRegistry,
            logger);
    }
}