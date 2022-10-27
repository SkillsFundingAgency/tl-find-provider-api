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
        var employerInterestBuilder = new EmployerInterestBuilder();
        var employerInterest = new EmployerInterestBuilder().Build();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var geoLocation = GeoLocationBuilder.BuildGeoLocation(employerInterest.Postcode);
        var expectedEmployerInterest = employerInterestBuilder.BuildWithGeoLocation(geoLocation);

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
        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}, {routes.Single(r => r.Id == 2).Name}";

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
                            { "organisation_name", employerInterest.OrganisationName },
                            { "contact_name", employerInterest.ContactName },
                            { "email_address", employerInterest.Email },
                            { "telephone", employerInterest.Telephone },
                            { "website", employerInterest.Website  },
                            { "contact_preference", expectedContactPreference },
                            { "primary_industry", expectedIndustry },
                            { "placement_area", expectedSkillAreas },
                            { "has_multiple_placement_areas", "yes" },
                            { "postcode", employerInterest.Postcode },
                            { "additional_information", employerInterest.AdditionalInformation },
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri }
                        })));
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

        const string expectedContactPreference = "No preference";
        var expectedIndustry =
            $"{industries.Single(i => i.Id == 9).Name}";

        var expectedSkillAreas =
            $"{routes.Single(r => r.Id == 1).Name}, {routes.Single(r => r.Id == 2).Name}";

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
                            { "organisation_name", string.Empty },
                            { "contact_name", string.Empty },
                            { "email_address", string.Empty },
                            { "telephone", string.Empty },
                            { "website", string.Empty },
                            { "contact_preference", expectedContactPreference },
                            { "primary_industry", string.Empty },
                            { "placement_area", string.Empty },
                            { "has_multiple_placement_areas", "no" },
                            { "postcode", employerInterest.Postcode },
                            { "additional_information", string.Empty },
                            { "employer_support_site", settings.EmployerSupportSiteUri },
                            { "employer_unsubscribe_uri", expectedUnsubscribeUri }
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


        var service = new EmployerInterestServiceBuilder()
            .Build(
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var results =
            (await service.FindEmployerInterest(latitude, longitude))
            .ToList();

        results.Should().BeEquivalentTo(employerInterestSummaryList);
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

        var service = new EmployerInterestServiceBuilder()
            .Build(
                postcodeLookupService: postcodeLookupService,
                employerInterestRepository: employerInterestRepository,
                employerInterestSettings: settings);

        var results =
            (await service.FindEmployerInterest(postcode))
            .ToList();

        results.Should().BeEquivalentTo(employerInterestSummaryList);
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

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var results =
            (await service.GetSummaryList())
            .ToList();
        results.Should().BeEquivalentTo(employerInterestSummaryList);
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

    [Fact]
    public void ServiceStartDate_Returns_Expected_Value()
    {
        var serviceStartDate = DateTime.Parse("2022-11-11");
        var expectedServiceStartDate = DateOnly.FromDateTime(serviceStartDate);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestSettings:
                new SettingsBuilder()
                    .BuildEmployerInterestSettings(
                        serviceStartDate: serviceStartDate));

        service.ServiceStartDate.Should().Be(expectedServiceStartDate);
    }
}