using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class EmployerInterestServiceTests
{
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
        var employerInterest = new EmployerInterestBuilder().Build();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode);

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(
                Arg.Any<string>())
            .Returns(geoLocation);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(Arg.Any<EmployerInterest>())
            .Returns((1, uniqueId));

        var service = new EmployerInterestServiceBuilder()
            .Build(
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        await employerInterestRepository
            .Received(1)
            .Create(Arg.Any<EmployerInterest>());

        var expectedEmployerInterest = new EmployerInterest
        {
            OrganisationName = employerInterest.OrganisationName,
            ContactName = employerInterest.ContactName,
            Postcode = geoLocation.Location,
            Latitude = geoLocation.Latitude,
            Longitude = geoLocation.Longitude,
            HasMultipleLocations = employerInterest.HasMultipleLocations,
            LocationCount = employerInterest.LocationCount,
            IndustryId = employerInterest.IndustryId,
            OtherIndustry = employerInterest.OtherIndustry,
            Email = employerInterest.Email,
            Telephone = employerInterest.Telephone,
            Website = employerInterest.Website,
            ContactPreferenceType = employerInterest.ContactPreferenceType,
            AdditionalInformation = employerInterest.AdditionalInformation,
        };

        await employerInterestRepository
            .Received(1)
            .Create(Arg.Is<EmployerInterest>(e =>
                e.Validate(expectedEmployerInterest, false, false)));
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
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(Arg.Any<EmployerInterest>())
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
                Arg.Any<IDictionary<string, string>>()
            );
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService_With_Expected_Tokens()
    {
        var employerInterest = new EmployerInterestBuilder().Build();

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
            .Returns(new IndustryBuilder().BuildList());
        providerDataService.GetRoutes()
            .Returns(new RouteBuilder().BuildList());

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Create(Arg.Any<EmployerInterest>())
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
        const string expectedIndustry = "IT and Communications";

        var expectedUnsubscribeUri =
            $"{settings.UnsubscribeEmployerUri.TrimEnd('/')}?id={uniqueId.ToString("D").ToLower()}";

        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                EmailTemplateNames.EmployerRegisterInterest,
                Arg.Is<IDictionary<string, string>>(tokens =>
                    tokens.ValidateTokens(
                        new Dictionary<string, string>
                        {
                            {"organisation_name", employerInterest.OrganisationName},
                            {"contact_name", employerInterest.ContactName},
                            {"email_address", employerInterest.Email},
                            {"telephone", employerInterest.Telephone},
                            {"website", employerInterest.Website },
                            {"contact_preference", expectedContactPreference},
                            {"primary_industry", expectedIndustry},
                            {"placement_area", "(TODO: placement area)"},
                            {"postcode", employerInterest.Postcode},
                            {"additional_information", employerInterest.AdditionalInformation},
                            {"employer_support_site", settings.EmployerSupportSiteUri},
                            {"employer_unsubscribe_uri", expectedUnsubscribeUri}
                        })));
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
    public async Task RemoveExpiredEmployerInterest_Does_Not_Call_Repository_For_Zero_RetentionDays()
    {
        var settings = new EmployerInterestSettings
        {
            RetentionDays = 0
        };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-13"));

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(0);

        var service = new EmployerInterestServiceBuilder()
            .Build(dateTimeService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(0);

        await employerInterestRepository
            .DidNotReceive()
            .DeleteBefore(Arg.Any<DateTime>());
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Calls_Repository()
    {
        var settings = new EmployerInterestSettings
        {
            RetentionDays = 12
        };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-13"));

        //Expected date is Today - RetentionDays 
        var expectedDate = DateTime.Parse("2022-08-01");

        const int count = 9;

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(count);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(count);

        await employerInterestRepository
            .Received(1)
            .DeleteBefore(Arg.Is<DateTime>(d => d == expectedDate));
    }

    [Fact]
    public async Task FindEmployerInterest_Returns_Expected_List()
    {
        var employerInterestSummaryList = new EmployerInterestSummaryBuilder()
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetSummaryList()
            .Returns(employerInterestSummaryList);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var results = (await service.FindEmployerInterest()).ToList();
        results.Should().BeEquivalentTo(employerInterestSummaryList);

        await employerInterestRepository
            .Received(1)
            .GetSummaryList();
    }

    [Fact]
    public async Task GetEmployerInterest_Returns_Expected_Value()
    {
        var employerInterest = new EmployerInterestBuilder()
            .Build();
        var id = employerInterest.Id;

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.Get(id)
            .Returns(employerInterest);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var result = await service.GetEmployerInterest(id);
        result.Should().Be(employerInterest);

        await employerInterestRepository
            .Received(1)
            .Get(id);
    }

    [Fact]
    public void RetentionDays_Returns_Expected_Value()
    {
        const int retentionDays = 99;

        var settings = new SettingsBuilder()
            .BuildEmployerInterestSettings(
                retentionDays: retentionDays);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestSettings: settings);

        service.RetentionDays.Should().Be(retentionDays);
    }
}