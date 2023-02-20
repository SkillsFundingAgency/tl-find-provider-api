using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class NotificationServiceTests
{
    private const int TestUkPrn = 10099099;

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(NotificationService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(NotificationService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }
    [Fact]
    public async Task CreateNotification_Calls_Repository()
    {
        const int newId = 1;

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository
            .Create(notification, TestUkPrn)
            .Returns(newId);

        var uniqueId = Guid.Parse("b4fd2a81-dcc9-43b9-9f4e-be76d1faa801");
        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
            .Returns(uniqueId);

        var service = new NotificationServiceBuilder()
            .Build(guidProvider: guidProvider,
                notificationRepository: notificationRepository);

        var result = await service.CreateNotification(notification, TestUkPrn);

        result.Should().Be(newId);

        await notificationRepository
            .Received(1)
            .Create(Arg.Is<Notification>(n =>
                    ReferenceEquals(n, notification) &&
                    n.EmailVerificationToken == uniqueId),
                TestUkPrn);
    }

    [Fact]
    public async Task CreateNotification_Sends_Email_Verification_Email()
    {
        var uniqueId = Guid.Parse("f0f7d306-f114-42c5-adeb-87b67e580f1a");

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var baseUri = providerSettings.ConnectSiteUri?.TrimEnd('/');
        var expectedNotificationsUri = $"{baseUri}/notifications";
        var expectedVerificationUri = $"{baseUri}/notifications?token={uniqueId:D}";

        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
        .Returns(uniqueId);

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var service = new NotificationServiceBuilder()
            .Build(
                guidProvider: guidProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.CreateNotification(notification, TestUkPrn);

        await emailService
            .Received(1)
            .SendEmail(
                notification.Email,
                EmailTemplateNames.ProviderVerification,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());

        await emailService
            .Received(1)
            .SendEmail(
                notification.Email,
                EmailTemplateNames.ProviderVerification,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_verification_link", expectedVerificationUri },
                            { "notifications_uri", expectedNotificationsUri }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task CreateNotificationLocation_Calls_Repository()
    {
        const int providerNotificationId = 10;
        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();

        var uniqueId = Guid.Parse("b4fd2a81-dcc9-43b9-9f4e-be76d1faa801");
        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
            .Returns(uniqueId);

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        await service.CreateNotificationLocation(notification, providerNotificationId);

        await notificationRepository
            .Received(1)
            .CreateLocation(notification, providerNotificationId);
    }

    [Fact]
    public async Task DeleteNotification_Calls_Repository()
    {
        const int id = 101;

        var notificationRepository = Substitute.For<INotificationRepository>();

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        await service.DeleteNotification(id);

        await notificationRepository
            .Received(1)
            .Delete(id);
    }

    [Fact]
    public async Task DeleteNotificationLocation_Calls_Repository()
    {
        const int id = 101;

        var notificationRepository = Substitute.For<INotificationRepository>();

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        await service.DeleteNotificationLocation(id);

        await notificationRepository
            .Received(1)
            .DeleteLocation(id);
    }

    [Fact]
    public async Task GetNotification_Returns_Expected_Item()
    {
        const int id = 1;

        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotification(id)
            .Returns(notification);

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        var result = await service.GetNotification(id);
        result.Should().BeEquivalentTo(notification);

        await notificationRepository
            .Received(1)
            .GetNotification(id);
    }

    [Fact]
    public async Task GetNotificationLocation_Returns_Expected_Item()
    {
        const int id = 1;

        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotificationLocation(id)
            .Returns(notification);

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        var result = await service.GetNotificationLocation(id);
        result.Should().BeEquivalentTo(notification);

        await notificationRepository
            .Received(1)
            .GetNotificationLocation(id);
    }

    [Fact]
    public async Task GetNotificationSummaryList_Returns_Expected_List()
    {
        const long ukPrn = 12345678;

        var notificationSummaries = new NotificationSummaryBuilder()
            .BuildList()
            .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotificationSummaryList(ukPrn, Arg.Any<bool>())
            .Returns(notificationSummaries);

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        var results = (await service.GetNotificationSummaryList(ukPrn)).ToList();
        results.Should().BeEquivalentTo(notificationSummaries);

        await notificationRepository
            .Received(1)
            .GetNotificationSummaryList(ukPrn, true);
    }

    [Fact]
    public async Task GetNotificationLocationSummaryList_Returns_Expected_List()
    {
        const int notificationId = 1;

        var notificationLocationSummaries = new NotificationLocationSummaryBuilder()
            .BuildList()
            .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotificationLocationSummaryList(notificationId)
            .Returns(notificationLocationSummaries);

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        var results = (await service.GetNotificationLocationSummaryList(notificationId)).ToList();
        results.Should().BeEquivalentTo(notificationLocationSummaries);

        await notificationRepository
        .Received(1)
            .GetNotificationLocationSummaryList(notificationId);
    }

    [Fact]
    public async Task GetAvailableNotificationLocationPostcodes_Returns_Expected_List()
    {
        const int providerNotificationId = 1;

        var locationNames = new NotificationLocationNameBuilder()
            .BuildList()
            .ToList();

        var expectedLocationNames = new NotificationLocationNameBuilder()
            .BuildListOfAvailableLocations()
            .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetProviderNotificationLocations(providerNotificationId)
            .Returns(locationNames);

        var service = new NotificationServiceBuilder().Build(
            notificationRepository: notificationRepository);

        var response = (await service
                .GetAvailableNotificationLocationPostcodes(providerNotificationId))
            ?.ToList();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(expectedLocationNames);
    }

    [Fact]
    public async Task SendProviderNotifications_Calls_EmailService()
    {
        const NotificationFrequency frequency = NotificationFrequency.Daily;

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var notificationEmails = new NotificationEmailBuilder()
            .BuildList()
            .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetPendingNotificationEmails(frequency)
            .Returns(notificationEmails);

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var service = new NotificationServiceBuilder()
            .Build(
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.SendProviderNotifications(frequency);

        await emailService
            .Received(notificationEmails.Count)
            .SendEmail(
                Arg.Any<string>(),
                EmailTemplateNames.ProviderNotification,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());

        await emailService
            .Received(1)
            .SendEmail(
                notificationEmails.First().Email,
                EmailTemplateNames.ProviderNotification,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task SendProviderNotifications_Does_Not_Call_EmailService_If_Already_Updated()
    {
        const NotificationFrequency frequency = NotificationFrequency.Daily;
        var notificationDate = DateTime.Parse("2023-02-13 11:32:42");

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(notificationDate);

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var notificationEmails = new NotificationEmailBuilder()
            .BuildList()
            .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetPendingNotificationEmails(frequency)
            .Returns(notificationEmails);
        notificationRepository.GetLastNotificationSentDate(Arg.Any<IEnumerable<int>>())
            .Returns(notificationDate);

        var emailService = Substitute.For<IEmailService>();

        var service = new NotificationServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.SendProviderNotifications(frequency);

        await emailService
            .DidNotReceive()
            .SendEmail(
                Arg.Any<string>(),
                EmailTemplateNames.ProviderNotification,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task SendProviderNotifications_Calls_UpdateNotificationSentDate()
    {
        const NotificationFrequency frequency = NotificationFrequency.Daily;
        var notificationDate = DateTime.Parse("2023-02-13 11:32:42");

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(notificationDate);

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var notificationEmails = new NotificationEmailBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var notificationIds =
            notificationEmails
                .Select(x => x.NotificationLocationId)
                .ToList();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetPendingNotificationEmails(frequency)
            .Returns(notificationEmails);

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var service = new NotificationServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.SendProviderNotifications(frequency);

        await notificationRepository
            .Received(notificationEmails.Count)
            .UpdateNotificationSentDate(
                Arg.Any<IEnumerable<int>>(),
                notificationDate);

        await notificationRepository
            .Received(1)
            .UpdateNotificationSentDate(
                Arg.Is<IEnumerable<int>>(
                    ids =>
                        ids.Single() == notificationIds.Single()),
                notificationDate);
    }

    [Fact]
    public async Task SendProviderNotificationEmail_Calls_EmailService()
    {
        const int notificationId = 1;
        var uniqueId = Guid.Parse("f0f7d306-f114-42c5-adeb-87b67e580f1a");

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var baseUri = providerSettings.ConnectSiteUri?.TrimEnd('/');
        var expectedNotificationsUri = $"{baseUri}/notifications";
        var expectedEmployerListUri = $"{baseUri}/employer-list";
        var expectedSearchFiltersUri = $"{baseUri}/filters";

        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
        .Returns(uniqueId);

        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotification(notificationId)
            .Returns(notification);

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var service = new NotificationServiceBuilder()
            .Build(
                guidProvider: guidProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.SendProviderNotificationEmail(notification.Email);

        await emailService
            .Received(1)
            .SendEmail(
                notification.Email,
                EmailTemplateNames.ProviderNotification,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());

        await emailService
            .Received(1)
            .SendEmail(
                notification.Email,
                EmailTemplateNames.ProviderNotification,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "employer_list_uri", expectedEmployerListUri },
                            { "search_filters_uri", expectedSearchFiltersUri },
                            { "notifications_uri", expectedNotificationsUri }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task SendProviderVerificationEmail_Calls_EmailService_And_Repository()
    {
        const int notificationId = 1;
        var uniqueId = Guid.Parse("f0f7d306-f114-42c5-adeb-87b67e580f1a");

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var baseUri = providerSettings.ConnectSiteUri?.TrimEnd('/');
        var expectedNotificationsUri = $"{baseUri}/notifications";
        var expectedVerificationUri = $"{baseUri}/notifications?token={uniqueId:D}";

        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
        .Returns(uniqueId);

        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();
        notificationRepository.GetNotification(notificationId)
            .Returns(notification);

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var service = new NotificationServiceBuilder()
            .Build(
                guidProvider: guidProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.SendProviderNotificationVerificationEmail(notificationId, notification.Email);

        await emailService
            .Received(1)
            .SendEmail(
                notification.Email,
                EmailTemplateNames.ProviderVerification,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "email_verification_link", expectedVerificationUri },
                            { "notifications_uri", expectedNotificationsUri }
                        })),
                Arg.Any<string>());

        await notificationRepository
            .Received(1)
            .SaveEmailVerificationToken(notificationId, notification.Email, uniqueId);
    }

    [Fact]
    public async Task UpdateNotification_Calls_Repository()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        await service.UpdateNotification(notification);

        await notificationRepository
            .Received(1)
            .Update(notification);
    }

    [Fact]
    public async Task UpdateNotification_Does_Not_Send_Email_Verification_Email()
    {
        var uniqueId = Guid.Parse("f0f7d306-f114-42c5-adeb-87b67e580f1a");

        var providerSettings = new SettingsBuilder()
            .BuildProviderSettings();

        var guidProvider = Substitute.For<IGuidProvider>();
        guidProvider
            .NewGuid()
        .Returns(uniqueId);

        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();

        var emailService = Substitute.For<IEmailService>();

        var service = new NotificationServiceBuilder()
            .Build(
                guidProvider: guidProvider,
                emailService: emailService,
                notificationRepository: notificationRepository,
                providerSettings: providerSettings);

        await service.UpdateNotification(notification);

        await emailService
            .DidNotReceiveWithAnyArgs()
            .SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task UpdateNotificationLocation_Calls_Repository()
    {
        var notification = new NotificationBuilder()
            .Build();

        var notificationRepository = Substitute.For<INotificationRepository>();

        var service = new NotificationServiceBuilder()
            .Build(notificationRepository: notificationRepository);

        await service.UpdateNotificationLocation(notification);

        await notificationRepository
            .Received(1)
            .UpdateLocation(notification);
    }

    [Fact]
    public async Task VerifyNotificationEmail_Calls_Repository()
    {
        const string token = "f0f7d306-f114-42c5-adeb-87b67e580f1a";
        var uniqueId = Guid.Parse(token);

        var notificationRepository = Substitute.For<INotificationRepository>();

        var service = new NotificationServiceBuilder()
            .Build(
                notificationRepository: notificationRepository);

        await service.VerifyNotificationEmail(token);

        await notificationRepository
            .Received(1)
            .VerifyEmailToken(uniqueId);
    }
}
