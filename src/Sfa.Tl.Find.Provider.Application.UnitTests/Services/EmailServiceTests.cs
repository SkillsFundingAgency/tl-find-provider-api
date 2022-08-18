using Sfa.Tl.Find.Provider.Application.Services;
using FluentAssertions;
using Notify.Interfaces;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class EmailServiceTests
{
    private const string TestEmailTemplateId = "52ffec27-201e-4b55-9ed5-470ddbda5331";
    private const string TestEmailTemplateName = "TestTemplate";
    private const string TestEmailAddress = "test@test-email.gov.uk";
    private const string MainSupportEmailInboxAddress = "support@test-email.gov.uk";
    private const string SecondarySupportEmailInboxAddress = "moresupport@test-email.gov.uk";

    private const string TestFullName = "Name";
    private const string TestPhone = "012-3456-78";
    private const string TestEmail = "test@test.com";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmailService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(EmailService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task EmailService_Sends_Email()
    {
        var notificationClient = Substitute.For<IAsyncNotificationClient>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(TestEmailTemplateId, TestEmailTemplateName);

        var emailService = new EmailServiceBuilder()
            .Build(emailTemplateRepository,
                notificationClient);

        var result = await emailService.SendEmail(
            TestEmailAddress,
            TestEmailTemplateName);

        result.Should().Be(true);

        await notificationClient
            .Received(1)
            .SendEmailAsync(Arg.Is<string>(emailAddress =>
                    emailAddress == TestEmailAddress),
                Arg.Is<string>(templateId =>
                    templateId == TestEmailTemplateId),
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Fact]
    public async Task EmailService_Sends_Email_With_Expected_Tokens()
    {
        var notificationClient = Substitute.For<IAsyncNotificationClient>();

        var emailTemplateRepository =new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(TestEmailTemplateId, TestEmailTemplateName);

        var personalisationTokens = new Dictionary<string, dynamic>
        {
            {"full_name", TestFullName},
            {"organisation_phone_number", TestPhone},
            {"organisation_email_address", TestEmail}
        };

        var emailService = new EmailServiceBuilder()
            .Build(emailTemplateRepository,
                notificationClient);

        var result = await emailService.SendEmail(
            TestEmailAddress,
            TestEmailTemplateName,
            personalisationTokens);

        result.Should().Be(true);

        await notificationClient
            .Received(1)
            .SendEmailAsync(Arg.Is<string>(emailAddress =>
                    emailAddress == TestEmailAddress),
                Arg.Is<string>(templateId =>
                    templateId == TestEmailTemplateId),
                Arg.Is<Dictionary<string, dynamic>>(tokens =>
                    tokens.HasExpectedValue("full_name", TestFullName) &&
                    tokens.HasExpectedValue("organisation_phone_number", TestPhone) &&
                    tokens.HasExpectedValue("organisation_email_address", TestEmail)));
    }

    [Fact]
    public async Task EmailService_Sends_No_Emails_When_Inbox_Address_Is_Empty()
    {
        var notificationClient = Substitute.For<IAsyncNotificationClient>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(TestEmailTemplateId, TestEmailTemplateName);

        var emailService = new EmailServiceBuilder()
            .Build(emailTemplateRepository,
                notificationClient);

        var result = await emailService.SendEmail(
            "",
            TestEmailTemplateName);

        result.Should().BeFalse();

        await notificationClient
            .DidNotReceive()
            .SendEmailAsync(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Fact]
    public async Task EmailService_Sends_No_Emails_When_Template_Is_Not_Found()
    {
        var notificationClient = Substitute.For<IAsyncNotificationClient>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(null, null);

        var emailService = new EmailServiceBuilder()
            .Build(emailTemplateRepository,
                notificationClient);

        var result = await emailService.SendEmail(
            TestEmailAddress,
            TestEmailTemplateName);

        result.Should().BeFalse();

        await notificationClient
            .DidNotReceive()
            .SendEmailAsync(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());
    }

    [Fact]
    public async Task EmailService_Sends_Emails_To_Two_Recipients()
    {
        var notificationClient = Substitute.For<IAsyncNotificationClient>();

        var emailTemplateRepository = new EmailTemplateRepositoryBuilder()
            .BuildSubstitute(TestEmailTemplateId, TestEmailTemplateName);

        var emailAddressList = $"{MainSupportEmailInboxAddress};{SecondarySupportEmailInboxAddress}";

        var emailService = new EmailServiceBuilder()
            .Build(emailTemplateRepository,
                notificationClient);

        var result = await emailService.SendEmail(
            emailAddressList,
            TestEmailTemplateName);

        result.Should().BeTrue();

        await notificationClient
            .Received(2)
            .SendEmailAsync(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());

        await notificationClient
            .Received(1)
            .SendEmailAsync(Arg.Is<string>(emailAddress =>
                    emailAddress == MainSupportEmailInboxAddress),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());

        await notificationClient
            .Received(1)
            .SendEmailAsync(Arg.Is<string>(emailAddress =>
                    emailAddress == MainSupportEmailInboxAddress),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, dynamic>>());
    }
}
