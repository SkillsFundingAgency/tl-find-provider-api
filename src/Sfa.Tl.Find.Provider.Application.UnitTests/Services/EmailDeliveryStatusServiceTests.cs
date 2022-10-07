using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class EmailDeliveryStatusServiceTests
{
    private const string TestEmailTemplateName = "TestTemplate";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(EmailDeliveryStatusService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(EmailDeliveryStatusService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Returns_Expected_Result_For_Delivered()
    {
        var emailDeliveryReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.Delivered)
            .Build();

        var emailService = Substitute.For<IEmailService>();

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build(emailService);

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliveryReceipt);

        result.Should().Be(0);

        await emailService
            .DidNotReceive()
            .SendEmail(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>());
        await emailService
            .DidNotReceive()
            .SendEmail(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Sends_Email_For_PermanentFailure()
    {
        var emailDeliveryReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.PermanentFailure)
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();
        var emailService = Substitute.For<IEmailService>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(
                emailDeliveryReceipt.TemplateId.ToString(),
                TestEmailTemplateName);

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build(emailService, emailTemplateRepository, emailSettings);

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliveryReceipt);

        result.Should().Be(1);

        await emailService
            .Received(1)
            .SendEmail(emailSettings.SupportEmailAddress,
                EmailTemplateNames.EmailDeliveryStatus,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_type", TestEmailTemplateName },
                            { "reference", emailDeliveryReceipt.Reference },
                            { "reason", "permanent failure" },
                            { "sender_username", emailDeliveryReceipt.To },
                        }
                    )));
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Sends_Email_For_TemporaryFailure()
    {
        var emailDeliveryReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.TemporaryFailure)
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();
        var emailService = Substitute.For<IEmailService>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(
                emailDeliveryReceipt.TemplateId.ToString(),
                TestEmailTemplateName);

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build(emailService, emailTemplateRepository, emailSettings);

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliveryReceipt);

        result.Should().Be(1);

        await emailService
            .Received(1)
            .SendEmail(emailSettings.SupportEmailAddress,
                EmailTemplateNames.EmailDeliveryStatus,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_type", TestEmailTemplateName },
                            { "reference", emailDeliveryReceipt.Reference },
                            { "reason", "temporary failure" },
                            { "sender_username", emailDeliveryReceipt.To },
                        }
                    )));
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Sends_TechnicalFailure()
    {
        var emailDeliveryReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.TechnicalFailure)
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();
        var emailService = Substitute.For<IEmailService>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(
                emailDeliveryReceipt.TemplateId.ToString(),
                TestEmailTemplateName);

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build(emailService, emailTemplateRepository, emailSettings);

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliveryReceipt);

        result.Should().Be(1);

        await emailService
            .Received(1)
            .SendEmail(emailSettings.SupportEmailAddress,
                EmailTemplateNames.EmailDeliveryStatus,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_type", TestEmailTemplateName },
                            { "reference", emailDeliveryReceipt.Reference },
                            { "reason", "technical failure" },
                            { "sender_username", emailDeliveryReceipt.To },
                        }
                    )));
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Shows_Unknown_For_Unknown_Template_Id()
    {
        var emailDeliveryReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.TechnicalFailure)
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();
        var emailService = Substitute.For<IEmailService>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(null, null);

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build(emailService, emailTemplateRepository, emailSettings);

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliveryReceipt);

        result.Should().Be(1);

        await emailService
            .Received(1)
            .SendEmail(emailSettings.SupportEmailAddress,
                EmailTemplateNames.EmailDeliveryStatus,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_type", $"Unknown template {emailDeliveryReceipt.TemplateId}" },
                            { "reference", emailDeliveryReceipt.Reference },
                            { "reason", "technical failure" },
                            { "sender_username", emailDeliveryReceipt.To },
                        }
                    )));
    }
}
