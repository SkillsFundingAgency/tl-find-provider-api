using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class EmployerInterestServiceTests
{
    private readonly DateTime _defaultDateToday = DateTime.Parse("2022-08-13");

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerInterestService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(EmployerInterestService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_Repository()
    {
        var employerInterestBuilder = new EmployerInterestBuilder();
        var employerInterest = new EmployerInterestBuilder().Build();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode);
        var expectedEmployerInterest = employerInterestBuilder.BuildWithGeoLocation(geoLocation);
        const int retentionDays = 10;
        var expectedExpiryDate = DateTime.Parse("2022-08-23 23:59:59.9999999");

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: retentionDays);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(geoLocation);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var service = new EmployerInterestServiceBuilder()
        .Build(
                dateTimeProvider,
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        await employerInterestRepository
            .Received(1)
            .Create(Arg.Is<EmployerInterest>(e =>
                e.Validate(expectedEmployerInterest,
                    false,
                    false,
                        false,
                    false,
                    false)),
                Arg.Is<GeoLocation>(g =>
                    g.Validate(geoLocation)),
                Arg.Is<DateTime>(dt =>
                    dt.Date == expectedExpiryDate.Date &&
                    dt == expectedExpiryDate
                    ));
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService()
    {
        var employerInterest = new EmployerInterestBuilder().Build();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService_With_Expected_Tokens()
    {
        var employerInterest = new EmployerInterestBuilder().Build();
        var routes = new RouteBuilder().BuildList().ToList();
        var industries = new IndustryBuilder().BuildList().ToList();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries()
            .Returns(industries);
        providerDataService.GetRoutes()
            .Returns(routes);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                postcodeLookupService: postcodeLookupService,
                providerDataService: providerDataService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        const string expectedContactPreference = "Email";
        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}" +
            $", {routes.Single(r => r.Id == 2).Name}";

        var expectedUnsubscribeUri =
            $"{settings.UnsubscribeEmployerUri?.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";

        var expectedDetails =
            $"* Name: {employerInterest.ContactName}\r\n" +
            $"* Email address: {employerInterest.Email}\r\n" +
            $"* Telephone: {employerInterest.Telephone}\r\n" +
            $"* How would you prefer to be contacted: {expectedContactPreference}\r\n" +
            $"* Organisation name: {employerInterest.OrganisationName}\r\n" +
            $"* Website: {employerInterest.Website}\r\n" +
            $"* Organisation’s primary industry: {expectedIndustry}\r\n" +
            $"* Industry placement areas: {expectedSkillAreas}\r\n" +
            $"* Location: {employerInterest.LocationName} - {employerInterest.Postcode}\r\n" +
            $"* Additional information: {employerInterest.AdditionalInformation}\r\n";

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri },
                            { "details_list", expectedDetails }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService_With_Expected_Tokens_With_Formatted_Postcode()
    {
        const string unformattedPostcode = "cv12wt";
        const string expectedPostcode = "CV1 2WT";

        var employerInterest = new EmployerInterestBuilder()
            .BuildWithGeoLocation(
                new GeoLocation
                {
                    Location = unformattedPostcode
                });

        var routes = new RouteBuilder().BuildList().ToList();
        var industries = new IndustryBuilder().BuildList().ToList();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(GeoLocationBuilder.BuildGeoLocation(expectedPostcode));

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries()
            .Returns(industries);
        providerDataService.GetRoutes()
            .Returns(routes);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                postcodeLookupService: postcodeLookupService,
                providerDataService: providerDataService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        const string expectedContactPreference = "Email";
        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}" +
            $", {routes.Single(r => r.Id == 2).Name}";

        var expectedUnsubscribeUri =
            $"{settings.UnsubscribeEmployerUri?.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";

        var expectedDetails =
            $"* Name: {employerInterest.ContactName}\r\n" +
            $"* Email address: {employerInterest.Email}\r\n" +
            $"* Telephone: {employerInterest.Telephone}\r\n" +
            $"* How would you prefer to be contacted: {expectedContactPreference}\r\n" +
            $"* Organisation name: {employerInterest.OrganisationName}\r\n" +
            $"* Website: {employerInterest.Website}\r\n" +
            $"* Organisation’s primary industry: {expectedIndustry}\r\n" +
            $"* Industry placement areas: {expectedSkillAreas}\r\n" +
            $"* Location: {expectedPostcode}\r\n" +
            $"* Additional information: {employerInterest.AdditionalInformation}\r\n";

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri },
                            { "details_list", expectedDetails }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService_With_Double_Line_Breaks_Replaced()
    {
        const string inputAdditionalInformation = "Hello\n\n\nWorld.\n\nHow are you?\n";
        const string expectedAdditionalInformation = "Hello\nWorld.\nHow are you?\n";

        var employerInterest = new EmployerInterestBuilder()
            .WithAdditionalInformation(inputAdditionalInformation)
            .Build();
        var routes = new RouteBuilder().BuildList().ToList();
        var industries = new IndustryBuilder().BuildList().ToList();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries()
            .Returns(industries);
        providerDataService.GetRoutes()
            .Returns(routes);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                postcodeLookupService: postcodeLookupService,
                providerDataService: providerDataService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokenContains(
                        "details_list",
                        $"Additional information: {expectedAdditionalInformation}")),
                Arg.Any<string>());
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService_With_Null_Non_Mandatory_Properties()
    {
        var employerInterest = new EmployerInterestBuilder().BuildWithEmptyNonMandatoryProperties();
        var routes = new RouteBuilder().BuildList().ToList();
        var industries = new IndustryBuilder().BuildList().ToList();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries()
            .Returns(industries);
        providerDataService.GetRoutes()
            .Returns(routes);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(
                Arg.Any<EmployerInterest>(),
                Arg.Any<GeoLocation>(),
                Arg.Any<DateTime>())
            .Returns((1, uniqueId));

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                postcodeLookupService: postcodeLookupService,
                providerDataService: providerDataService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}";

        var expectedUnsubscribeUri =
             $"{settings.UnsubscribeEmployerUri?.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";
        var expectedDetails =
            $"* Name: {employerInterest.ContactName}\r\n" +
            $"* Email address: {employerInterest.Email}\r\n" +
            $"* Organisation name: {employerInterest.OrganisationName}\r\n" +
            $"* Organisation’s primary industry: {expectedIndustry}\r\n" +
            $"* Industry placement area: {expectedSkillAreas}\r\n" +
            $"* Location: {employerInterest.Postcode}\r\n";

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri },
                            { "details_list", expectedDetails }
                        })),
                uniqueId.ToString());
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_PostcodeLookupService()
    {
        var employerInterest = new EmployerInterestBuilder().Build();

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(geoLocation);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                postcodeLookupService: postcodeLookupService);

        await service.CreateEmployerInterest(employerInterest);

        await postcodeLookupService
            .Received(1)
            .GetPostcode(
                employerInterest.Postcode);
    }

    [Fact]
    public async Task DeleteEmployerInterest_By_Id_Calls_Repository()
    {
        var id = 101;
        const int count = 1;

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Delete(id)
            .Returns(count);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var result = await service.DeleteEmployerInterest(id);

        result.Should().Be(count);

        await employerInterestRepository
            .Received(1)
            .Delete(id);
    }

    [Fact]
    public async Task DeleteEmployerInterest_By_UniqueId_Calls_Repository()
    {
        var uniqueId = Guid.Parse("5AF374D2-1072-4E98-91CF-6AE765044DBA");
        const int count = 1;

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Delete(uniqueId)
            .Returns(count);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var result = await service.DeleteEmployerInterest(uniqueId);

        result.Should().Be(count);

        await employerInterestRepository
            .Received(1)
            .Delete(uniqueId);
    }

    [Fact]
    public async Task ExtendEmployerInterest_By_UniqueId_Calls_Repository()
    {
        var uniqueId = Guid.Parse("5AF374D2-1072-4E98-91CF-6AE765044DBA");
        var extensionResult = new ExtensionResultBuilder().Build();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.ExtendExpiry(
                uniqueId,
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(extensionResult);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.ExtendEmployerInterest(uniqueId);

        result.Success.Should().BeTrue();
        result.ExtensionsRemaining.Should().Be(extensionResult.ExtensionsRemaining);

        await employerInterestRepository
            .Received(1)
            .ExtendExpiry(
                uniqueId,
                settings.RetentionDays,
                settings.ExpiryNotificationDays,
                settings.MaximumExtensions);
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Does_Not_Call_Repository_For_Zero_RetentionDays()
    {
        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: 0);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();

        var service = new EmployerInterestServiceBuilder()
            .Build(dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(0);

        await employerInterestRepository
            .DidNotReceive()
            .GetExpiringInterest(Arg.Any<int>());
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Returns_Expected_Results_With_maximumExtensions()
    {
        const int maximumExtensions = 10;

        var employerInterestList = new EmployerInterestBuilder()
            .WithExtensionCounts(new[] { maximumExtensions -1, maximumExtensions } )
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            expiryNotificationDays: 7,
            retentionDays: 12,
            maximumExtensions: maximumExtensions);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetExpiringInterest(Arg.Any<int>())
            .Returns(employerInterestList);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(1);

        await employerInterestRepository
            .Received(1)
            .GetExpiringInterest(
                Arg.Is<int>(d =>
                    d == settings.ExpiryNotificationDays));
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Calls_EmailService()
    {
        var employerInterestList = new EmployerInterestBuilder()
            .WithUniqueId(Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC"))
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
             expiryNotificationDays: 7,
             retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        foreach (var employerInterest in employerInterestList)
        {
            postcodeLookupService.GetPostcode(
                    employerInterest.Postcode)
                .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));
        }

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetExpiringInterest(Arg.Any<int>())
            .Returns(employerInterestList);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService,
                postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(employerInterestList.Count);

        await emailService
            .Received(1)
            .SendEmail(
                employerInterestList.First().Email,
                EmailTemplateNames.EmployerExtendInterest,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Calls_EmailService_With_Expected_Tokens()
    {
        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var employerInterestList = new EmployerInterestBuilder()
            .WithUniqueId(uniqueId)
            .BuildList()
            .Take(1)
            .ToList();

        var routes = new RouteBuilder().BuildList().ToList();
        var industries = new IndustryBuilder().BuildList().ToList();

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        foreach (var employerInterest in employerInterestList)
        {
            postcodeLookupService.GetPostcode(
                    employerInterest.Postcode)
                .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));
        }

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.GetIndustries()
            .Returns(industries);
        providerDataService.GetRoutes()
            .Returns(routes);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetExpiringInterest(Arg.Any<int>())
            .Returns(employerInterestList);

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            expiryNotificationDays: 7,
            retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService,
                postcodeLookupService,
                providerDataService: providerDataService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(employerInterestList.Count);

        const string expectedContactPreference = "Email";
        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}" +
            $", {routes.Single(r => r.Id == 2).Name}";

        var expectedExtendUri =
            $"{settings.ExtendEmployerUri?.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";
        var expectedUnsubscribeUri =
            $"{settings.UnsubscribeEmployerUri?.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";

        var firstEmployerInterest = employerInterestList.First();

        var expectedDetails =
            $"* Name: {firstEmployerInterest.ContactName}\r\n" +
            $"* Email address: {firstEmployerInterest.Email}\r\n" +
            $"* Telephone: {firstEmployerInterest.Telephone}\r\n" +
            $"* How would you prefer to be contacted: {expectedContactPreference}\r\n" +
            $"* Organisation name: {firstEmployerInterest.OrganisationName}\r\n" +
            $"* Website: {firstEmployerInterest.Website}\r\n" +
            $"* Organisation’s primary industry: {expectedIndustry}\r\n" +
            $"* Industry placement areas: {expectedSkillAreas}\r\n" +
            $"* Location: {firstEmployerInterest.LocationName} - {firstEmployerInterest.Postcode}\r\n" +
            $"* Additional information: {firstEmployerInterest.AdditionalInformation}\r\n";

        await emailService
            .Received(1)
            .SendEmail(
                firstEmployerInterest.Email,
                EmailTemplateNames.EmployerExtendInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "details_list", expectedDetails },
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri },
                            { "employer_extend_uri", expectedExtendUri }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Calls_Repository()
    {
        var employerInterestList = new EmployerInterestBuilder()
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            expiryNotificationDays: 7,
            retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetExpiringInterest(Arg.Any<int>())
            .Returns(employerInterestList);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(employerInterestList.Count);

        await employerInterestRepository
            .Received(1)
            .GetExpiringInterest(
                Arg.Is<int>(d =>
                    d == settings.ExpiryNotificationDays));
    }

    [Fact]
    public async Task NotifyExpiringEmployerInterest_Calls_UpdateExtensionEmailSentDate()
    {
        var employerInterestList = new EmployerInterestBuilder()
            .WithUniqueId(Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC"))
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
             expiryNotificationDays: 7,
             retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        foreach (var employerInterest in employerInterestList)
        {
            postcodeLookupService.GetPostcode(
                    Arg.Any<string>())
                .Returns(GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode));
        }

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetExpiringInterest(Arg.Any<int>())
            .Returns(employerInterestList);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.NotifyExpiringEmployerInterest();

        result.Should().Be(employerInterestList.Count);

        await employerInterestRepository
            .Received(1)
            .UpdateExtensionEmailSentDate(
                employerInterestList[0].Id);
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Does_Not_Call_Repository_For_Zero_RetentionDays()
    {
        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: 0);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();

        var service = new EmployerInterestServiceBuilder()
            .Build(dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(0);

        await employerInterestRepository
            .DidNotReceive()
            .DeleteExpired(Arg.Any<DateTime>());
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Calls_Repository()
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var expiredEmployerInterest = new ExpiredEmployerInterestDtoBuilder()
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.DeleteExpired(Arg.Any<DateTime>())
            .Returns(expiredEmployerInterest);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(expiredEmployerInterest.Count);

        await employerInterestRepository
            .Received(1)
            .DeleteExpired(Arg.Is<DateTime>(d => d == _defaultDateToday));
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Calls_SendEmail()
    {
        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var expiredEmployerInterest = new ExpiredEmployerInterestDtoBuilder()
            .BuildList()
            .Take(2)
            .ToList();

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.DeleteExpired(Arg.Any<DateTime>())
            .Returns(expiredEmployerInterest);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(expiredEmployerInterest.Count);

        await emailService
            .Received(2)
            .SendEmail(
                Arg.Any<string>(),
                EmailTemplateNames.EmployerInterestRemoved,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());

        await emailService
            .Received(1)
            .SendEmail(
                expiredEmployerInterest[0].Email,
                EmailTemplateNames.EmployerInterestRemoved,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());

        await emailService
            .Received(1)
            .SendEmail(
                expiredEmployerInterest[1].Email,
                EmailTemplateNames.EmployerInterestRemoved,
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>());
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Calls_SendEmail_With_Expected_Tokens()
    {
        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: 12);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var expiredEmployerInterest = new ExpiredEmployerInterestDtoBuilder()
            .BuildList()
            .Take(1)
            .ToList();

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>(),
                Arg.Any<string>())
            .Returns(true);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.DeleteExpired(Arg.Any<DateTime>())
            .Returns(expiredEmployerInterest);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                emailService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(expiredEmployerInterest.Count);

        await emailService
            .Received(1)
            .SendEmail(
                expiredEmployerInterest[0].Email,
                EmailTemplateNames.EmployerInterestRemoved,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            { "organisation_name", expiredEmployerInterest[0].OrganisationName ?? "" },
                            { "postcode", expiredEmployerInterest[0].Postcode ?? "" },
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "register_interest_uri", settings.RegisterInterestUri ?? "" }
                        })),
                Arg.Any<string>());
    }

    [Fact]
    public async Task FindEmployerInterest_By_Lat_Long_Returns_Expected_List()
    {
        const double latitude = 52.400997;
        const double longitude = -1.508122;
        const int employerInterestsCount = 1000;

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
        .BuildList()
        .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                latitude,
                longitude,
                settings.SearchRadius
            )
            .Returns((employerInterestSummaryList, employerInterestsCount));

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (searchResults, count) =
            await service.FindEmployerInterest(latitude, longitude);

        searchResults.Should().BeEquivalentTo(employerInterestSummaryList);
        count.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task FindEmployerInterest_By_Lat_Long_Returns_Expected_List_Sets_New_And_Expiry()
    {
        const double latitude = 52.400997;
        const double longitude = -1.508122;
        const int employerInterestsCount = 1000;

        const int daysToRetain = 10;
        var today = DateTime.Parse("2022-12-07");
        var creationDate = DateTime.Parse("2022-12-01 11:30");
        var expiryDate = DateTime.Parse("2022-12-11");

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .WithCreationDate(creationDate)
            .WithExpiryDate(expiryDate)
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: daysToRetain);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(today);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                latitude,
                longitude,
                settings.SearchRadius
            )
            .Returns((employerInterestSummaryList, employerInterestsCount));

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (employerInterestSummaries, count) =
            await service.FindEmployerInterest(latitude, longitude);

        var searchResults = employerInterestSummaries
            .ToList();
        searchResults.Should().NotBeNullOrEmpty();
        searchResults.First().IsExpiring.Should().BeTrue();
        searchResults.First().IsNew.Should().BeTrue();
        count.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task FindEmployerInterest_By_Location_Returns_Expected_List()
    {
        const int locationId = 1;
        const int employerInterestsCount = 1000;
        const bool searchFiltersApplied = true;

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
        .BuildList()
        .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                locationId,
                settings.SearchRadius
            )
            .Returns((employerInterestSummaryList, employerInterestsCount, searchFiltersApplied));

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (searchResults, count, filtersApplied) =
            await service.FindEmployerInterest(locationId);

        searchResults.Should().BeEquivalentTo(employerInterestSummaryList);
        count.Should().Be(employerInterestsCount);
        filtersApplied.Should().Be(searchFiltersApplied);
    }

    [Fact]
    public async Task FindEmployerInterest_By_Location_Returns_Expected_List_Sets_New_And_Expiry()
    {
        const int locationId = 1;
        const int employerInterestsCount = 1000;
        const bool searchFiltersApplied = true;

        const int daysToRetain = 10;
        var today = DateTime.Parse("2022-12-07");
        var creationDate = DateTime.Parse("2022-12-01 11:30");
        var expiryDate = DateTime.Parse("2022-12-11");

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .WithCreationDate(creationDate)
            .WithExpiryDate(expiryDate)
            .BuildList()
            .ToList();

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: daysToRetain);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(today);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                locationId,
                settings.SearchRadius
            )
            .Returns((employerInterestSummaryList, employerInterestsCount, searchFiltersApplied));

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (employerInterestSummaries, count, filtersApplied) =
            await service.FindEmployerInterest(locationId);

        var searchResults = employerInterestSummaries
            .ToList();
        searchResults.Should().NotBeNullOrEmpty();
        searchResults.First().IsExpiring.Should().BeTrue();
        searchResults.First().IsNew.Should().BeTrue();
        count.Should().Be(employerInterestsCount);
        filtersApplied.Should().Be(searchFiltersApplied);
    }

    [Fact]
    public async Task FindEmployerInterest_By_Postcode_Returns_Expected_List()
    {
        const string postcode = "CV1 2WT";
        const int employerInterestsCount = 1000;

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(postcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                postcode)
            .Returns(geoLocation);

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                geoLocation.Latitude,
                geoLocation.Longitude,
                settings.SearchRadius
                )
            .Returns((employerInterestSummaryList, employerInterestsCount));

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (employerInterestSummaries, count) =
            await service.FindEmployerInterest(postcode);

        var searchResults = employerInterestSummaries
            .ToList();
        searchResults.Should().BeEquivalentTo(employerInterestSummaryList);
        count.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task FindEmployerInterest_By_Postcode_Sets_New_And_Expiry()
    {
        const string postcode = "CV1 2WT";
        const int employerInterestsCount = 1000;

        const int daysToRetain = 10;
        var today = DateTime.Parse("2022-12-07");
        var creationDate = DateTime.Parse("2022-12-01 11:30");
        var expiryDate = DateTime.Parse("2022-12-11");

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .WithCreationDate(creationDate)
            .WithExpiryDate(expiryDate)
            .BuildList()
            .ToList();

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(postcode);
        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                postcode)
            .Returns(geoLocation);

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: daysToRetain);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(today);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository
            .Search(
                geoLocation.Latitude,
                geoLocation.Longitude,
                settings.SearchRadius
            )
            .Returns((employerInterestSummaryList, employerInterestsCount));

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var (employerInterestSummaries, count) =
            await service.FindEmployerInterest(postcode);

        var searchResults = employerInterestSummaries
            .ToList();
        searchResults.Should().NotBeNullOrEmpty();
        searchResults.First().IsExpiring.Should().BeTrue();
        searchResults.First().IsNew.Should().BeTrue();
        count.Should().Be(employerInterestsCount);
    }

    [Fact]
    public async Task GetEmployerInterestDetail_Returns_Expected_Value()
    {
        var employerInterest = new EmployerInterestDetailBuilder()
            .Build();
        var id = employerInterest.Id;

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetDetail(id)
            .Returns(employerInterest);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var result = await service.GetEmployerInterestDetail(id);
        result.Should().Be(employerInterest);
    }

    [Fact]
    public async Task GetSummaryList_Returns_Expected_List()
    {
        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetSummaryList()
            .Returns(employerInterestSummaryList);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(_defaultDateToday);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository);

        var results =
            (await service.GetSummaryList())
            .ToList();
        results.Should().BeEquivalentTo(employerInterestSummaryList);
    }

    [Fact]
    public async Task GetSummaryList_Sets_New_And_Expiry()
    {
        const int daysToRetain = 10;
        var today = DateTime.Parse("2022-12-07");
        var creationDate = DateTime.Parse("2022-12-01 11:30");
        var expiryDate = DateTime.Parse("2022-12-11");

        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .WithCreationDate(creationDate)
            .WithExpiryDate(expiryDate)
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetSummaryList()
            .Returns(employerInterestSummaryList);

        var settings = new SettingsBuilder().BuildEmployerInterestSettings(
            retentionDays: daysToRetain);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.Today.Returns(today);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeProvider,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var results =
            (await service.GetSummaryList())
            .ToList();
        results.Should().BeEquivalentTo(employerInterestSummaryList);

        results.Should().NotBeNullOrEmpty();
        results.First().ExpiryDate.Should().Be(expiryDate);
        results.First().IsExpiring.Should().BeTrue();
        results.First().IsNew.Should().BeTrue();

    }

    [Fact]
    public void RetentionDays_Returns_Expected_Value()
    {
        const int retentionDays = 99;

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestSettings:
                new SettingsBuilder()
                    .BuildEmployerInterestSettings(
                        retentionDays: retentionDays));

        service.RetentionDays.Should().Be(retentionDays);
    }
}