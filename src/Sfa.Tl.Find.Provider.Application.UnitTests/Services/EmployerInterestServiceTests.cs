using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using static Dapper.SqlMapper;

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

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.Create(employerInterest)
            .Returns(uniqueId);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: repository);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        await repository
            .Received(1)
            .Create(employerInterest);
    }

    [Fact]
    public async Task CreateEmployerInterest_Calls_EmailService()
    {
        var employerInterest = new EmployerInterestBuilder().Build();

        var uniqueId = Guid.Parse("916ED6B3-DF1D-4E03-9E7F-32BFD13583FC");

        var emailService = Substitute.For<IEmailService>();
        emailService.SendEmail(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IDictionary<string, string>>())
            .Returns(true);

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.Create(employerInterest)
            .Returns(uniqueId);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                employerInterestRepository: repository);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        //TODO: use EmailTemplateNames.EmployerRegisterInterest
        const string templateName = "EmployerRegisterInterest";
        
        await emailService
                .Received(1)
                .SendEmail(
                    employerInterest.Email,
                    templateName, //EmailTemplateNames.EmployerRegisterInterest
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

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.Create(employerInterest)
            .Returns(uniqueId);

        var settings = new SettingsBuilder().BuildEmployerInterestSettings();

        var service = new EmployerInterestServiceBuilder()
            .Build(
                emailService: emailService,
                employerInterestRepository: repository,
                employerInterestSettings: settings);

        var result = await service.CreateEmployerInterest(employerInterest);

        result.Should().Be(uniqueId);

        var formattedUniqueId = employerInterest.UniqueId.ToString("D").ToLower();
        var expectedUnsubscribeUri =
            $"{settings.EmployerSupportSiteUri}{(settings.EmployerSupportSiteUri.EndsWith('/') ? "" : "/")}{formattedUniqueId}";

        //TODO: use EmailTemplateNames.EmployerRegisterInterest
        const string templateName = "EmployerRegisterInterest";
        await emailService
            .Received(1)
            .SendEmail(
                employerInterest.Email,
                templateName, //EmailTemplateNames.EmployerRegisterInterest
                Arg.Is<IDictionary<string, string>>(tokens =>
                    //TODO: Use the shared token validation class
                    //tokens.ValidateTokens(
                    ValidateTokens(tokens, new Dictionary<string, string>
                    {
                        { "organisation_name", employerInterest.OrganisationName },
                        { "contact_name", employerInterest.ContactName },
                        { "email_address", employerInterest.Email },
                        { "telephone", employerInterest.Telephone },
                        //Need to convert to text 1=Email, 2=Telephone
                        { "contact_preference", employerInterest.ContactPreferenceType.ToString() },
                        //Need to convert to text
                        { "primary_industry", employerInterest.IndustryId.ToString() },
                        { "postcode", employerInterest.Postcode },
                        { "specific_requirements", employerInterest.SpecificRequirements },
                        //{ "how_many_locations", employerInterest.HasMultipleLocations + employerInterest.LocationCount }, //A single location
                        { "how_many_locations", employerInterest.LocationCount.ToString() }, //A single location
                        //TODO: Get this into config - EmployerInterestSettings_EmployerSupportSiteUri
                        //{ "employer_support_site", employerInterestConfiguration.EmployerSupportSiteUri },
                        { "employer_support_site", settings.EmployerSupportSiteUri },
                        { "employer_unsubscribe_uri", expectedUnsubscribeUri }
                    })));
    }

    private static bool ValidateTokens(IDictionary<string, string> tokens, IDictionary<string, string> expectedTokens)
    {
        foreach (var (key, value) in expectedTokens)
        {
            tokens.Should().ContainKey(key);

            if (key == "")
            {
            }
            tokens[key].Should().Be(value,
                $"this is the expected value for key '{key}'");
        }

        return true;
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

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(0);

        var service = new EmployerInterestServiceBuilder()
            .Build(dateTimeService,
                employerInterestRepository: repository,
                employerInterestSettings:settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(0);

        await repository
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

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(count);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeService,
                employerInterestRepository: repository,
                employerInterestSettings: settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(count);

        await repository
            .Received(1)
            .DeleteBefore(Arg.Is<DateTime>(d => d == expectedDate));
    }

    [Fact]
    public async Task FindEmployerInterest_Returns_Expected_List()
    {
        var employerInterests = new EmployerInterestBuilder()
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetAll()
            .Returns(employerInterests);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var results = (await service.FindEmployerInterest()).ToList();
        results.Should().BeEquivalentTo(employerInterests);

        await employerInterestRepository
            .Received(1)
            .GetAll();
    }
}