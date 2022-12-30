using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class CourseDirectoryServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(CourseDirectoryService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(CourseDirectoryService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task ImportProviders_Creates_Expected_Providers()
    {
        var responses = new Dictionary<string, string>
        {
            { CourseDirectoryService.CourseDetailEndpoint, CourseDirectoryJsonBuilder.BuildValidTLevelsResponse() }
        };

        IList<Application.Models.Provider> receivedProviders = null;

        var providerRepository = Substitute.For<IProviderRepository>();
        await providerRepository.Save(Arg.Do<IList<Application.Models.Provider>>(
            x => receivedProviders = x?.ToList()));

        var service = new CourseDirectoryServiceBuilder()
            .Build(responses, providerRepository);

        await service.ImportProviders();

        receivedProviders.Should().NotBeNull();
        receivedProviders.Count.Should().Be(1);

        var targetProvider = receivedProviders.SingleOrDefault(p => p.UkPrn == 10000055);
        targetProvider.Should().NotBeNull();

        targetProvider!.UkPrn.Should().Be(10000055);
        targetProvider.Name.Should().BeEquivalentTo("ABINGDON AND WITNEY COLLEGE");

        targetProvider.Validate(
            10000055,
            "ABINGDON AND WITNEY COLLEGE",
            "Wootton Road",
            null,
            "Abingdon",
            "Oxfordshire",
            "OX14 1GG",
            "test.user@abingdon-witney.ac.uk",
            "01234 555555",
            "http://www.abingdon-witney.ac.uk",
            locationCount: 2);

        var abingdonLocation = targetProvider.Locations.Single(l => l.Postcode == "OX14 1GG");
        abingdonLocation.Validate(
            "ABINGDON CAMPUS",
            "OX14 1GG",
            "Wootton Road",
            "Northcourt",
            "Abingdon",
            "Oxfordshire",
            "enquiry@abingdon-witney.ac.uk",
            "01234 555555",
            "http://www.abingdon-witney.ac.uk",
            51.680637,
            -1.286943,
            1);

        abingdonLocation.DeliveryYears.First().Validate(2021, new[] { 37, 38, 41 });
        var witneyLocation = targetProvider.Locations.Single(l => l.Postcode == "OX28 6NE");

        witneyLocation.Validate(
            "WITNEY CAMPUS",
            "OX28 6NE",
            "Holloway Road",
            "",
            "Witney",
            "Oxfordshire",
            "enquiry@abingdon-witney.ac.uk",
            "01993 777777",
            "http://www.abingdon-witney.ac.uk",
            51.785444,
            -1.487948,
            1);

        witneyLocation.DeliveryYears.First().Validate(2021, new[] { 37, 38, 41 });
    }

    [Fact]
    public async Task ImportQualifications_Creates_Expected_Qualifications()
    {
        var responses = new Dictionary<string, string>
        {
            { CourseDirectoryService.QualificationsEndpoint, CourseDirectoryJsonBuilder.BuildValidTLevelDefinitionsResponse() }
        };

        IList<Qualification> receivedQualifications = null;

        var qualificationRepository = Substitute.For<IQualificationRepository>();
        await qualificationRepository
            .Save(Arg.Do<IEnumerable<Qualification>>(
                x => receivedQualifications = x?.ToList()));

        var service = new CourseDirectoryServiceBuilder()
            .Build(responses, qualificationRepository: qualificationRepository);

        await service.ImportQualifications();

        receivedQualifications.Should().NotBeNull();
        receivedQualifications.Count.Should().Be(16);

        receivedQualifications.Should().Contain(q => q.Id == 36 && q.Name == "Design, Surveying and Planning for Construction");
        receivedQualifications.Should().Contain(q => q.Id == 37 && q.Name == "Digital Production, Design and Development");
        receivedQualifications.Should().Contain(q => q.Id == 38 && q.Name == "Education and Childcare");
        receivedQualifications.Should().Contain(q => q.Id == 39 && q.Name == "Digital Business Services");
        receivedQualifications.Should().Contain(q => q.Id == 40 && q.Name == "Digital Support Services");
        receivedQualifications.Should().Contain(q => q.Id == 41 && q.Name == "Health");
        receivedQualifications.Should().Contain(q => q.Id == 42 && q.Name == "Healthcare Science");
        receivedQualifications.Should().Contain(q => q.Id == 43 && q.Name == "Science");
        receivedQualifications.Should().Contain(q => q.Id == 44 && q.Name == "Onsite Construction");
        receivedQualifications.Should().Contain(q => q.Id == 45 && q.Name == "Building Services Engineering for Construction");
        receivedQualifications.Should().Contain(q => q.Id == 46 && q.Name == "Finance");
        receivedQualifications.Should().Contain(q => q.Id == 47 && q.Name == "Accounting");
        receivedQualifications.Should().Contain(q => q.Id == 48 && q.Name == "Design and Development for Engineering and Manufacturing");
        receivedQualifications.Should().Contain(q => q.Id == 49 && q.Name == "Maintenance, Installation and Repair for Engineering and Manufacturing");
        receivedQualifications.Should().Contain(q => q.Id == 50 && q.Name == "Engineering, Manufacturing, Processing and Control");
        receivedQualifications.Should().Contain(q => q.Id == 51 && q.Name == "Management and Administration");
    }

    [Fact]
    public async Task ImportProviders_Clears_Caches()
    {
        var responses = new Dictionary<string, string>
        {
            { CourseDirectoryService.CourseDetailEndpoint, CourseDirectoryJsonBuilder.BuildValidTLevelsResponse() }
        };

        var cacheService = Substitute.For<ICacheService>();

        var service = new CourseDirectoryServiceBuilder()
            .Build(responses,
                cacheService: cacheService);

        await service.ImportProviders();

        await cacheService
            .Received(1)
            .Remove<IList<Qualification>>(CacheKeys.QualificationsKey);
        await cacheService
            .Received(1)
            .Remove<IList<Route>>(CacheKeys.RoutesKey);
        await cacheService
            .Received(1)
            .Remove<ProviderDataDownloadInfoResponse>(CacheKeys.ProviderDataDownloadInfoKey);
    }

    [Fact]
    public async Task ImportQualifications_Clears_Caches()
    {
        var responses = new Dictionary<string, string>
        {
            { CourseDirectoryService.QualificationsEndpoint, CourseDirectoryJsonBuilder.BuildValidTLevelDefinitionsResponse() }
        };

        var cacheService = Substitute.For<ICacheService>();

        var service = new CourseDirectoryServiceBuilder()
            .Build(responses,
                cacheService: cacheService);

        await service.ImportQualifications();

        await cacheService
            .Received(1)
            .Remove<IList<Qualification>>(CacheKeys.QualificationsKey);
        await cacheService
            .Received(1)
            .Remove<IList<Route>>(CacheKeys.RoutesKey);
        await cacheService
            .Received(1)
            .Remove<ProviderDataDownloadInfoResponse>(CacheKeys.ProviderDataDownloadInfoKey);
    }
}