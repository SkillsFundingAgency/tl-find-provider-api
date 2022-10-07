using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Microsoft.VisualBasic.FileIO;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Csv;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class ProviderDataServiceTests
{
    private const int TestPage = 3;
    private const int TestPageSize = Constants.DefaultPageSize + 10;
    private const int TestUkPrn = 10099099;

    private readonly IList<int> _testRouteIds = new List<int> { 6, 7 };
    private readonly IList<int> _testQualificationIds = new List<int> { 37, 40, 51 };

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(ProviderDataService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(ProviderDataService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task GetIndustries_Returns_Expected_List()
    {
        var industries = new IndustryBuilder()
            .BuildList()
            .ToList();

        var industryRepository = Substitute.For<IIndustryRepository>();
        industryRepository.GetAll()
            .Returns(industries);

        var service = new ProviderDataServiceBuilder()
            .Build(industryRepository: industryRepository);

        var results = (await service.GetIndustries()).ToList();
        results.Should().BeEquivalentTo(industries);

        await industryRepository
            .Received(1)
            .GetAll();
    }

    [Fact]
    public async Task GetIndustries_Returns_Expected_List_From_Cache()
    {
        var industries = new IndustryBuilder().BuildList();

        var industryRepository = Substitute.For<IIndustryRepository>();

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Industry>>())
            .Returns(x =>
            {
                if ((string)x[0] == CacheKeys.IndustriesKey)
                {
                    x[1] = industries;
                    return true;
                }

                return false;
            });

        var service = new ProviderDataServiceBuilder()
            .Build(industryRepository: industryRepository,
                cache: cache);

        var results = await service.GetIndustries();
        results.Should().BeEquivalentTo(industries);

        await industryRepository
            .DidNotReceive()
            .GetAll();
    }

    [Fact]
    public async Task GetQualifications_Returns_Expected_List()
    {
        var qualifications = new QualificationBuilder()
            .BuildList()
            .ToList();

        var qualificationRepository = Substitute.For<IQualificationRepository>();
        qualificationRepository.GetAll()
            .Returns(qualifications);

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository);

        var results = (await service.GetQualifications()).ToList();
        results.Should().BeEquivalentTo(qualifications);

        await qualificationRepository
            .Received(1)
            .GetAll();
    }

    [Fact]
    public async Task GetQualifications_Returns_Expected_List_From_Cache()
    {
        var qualifications = new QualificationBuilder().BuildList();

        var qualificationRepository = Substitute.For<IQualificationRepository>();

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Qualification>>())
            .Returns(x =>
            {
                if ((string)x[0] == CacheKeys.QualificationsKey)
                {
                    x[1] = qualifications;
                    return true;
                }

                return false;
            });

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository,
                cache: cache);

        var results = await service.GetQualifications();
        results.Should().BeEquivalentTo(qualifications);

        await qualificationRepository
            .DidNotReceive()
            .GetAll();
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var routeRepository = Substitute.For<IRouteRepository>();
        routeRepository.GetAll(true)
            .Returns(routes);

        var service = new ProviderDataServiceBuilder()
            .Build(routeRepository: routeRepository);

        var results = (await service.GetRoutes()).ToList();
        results.Should().BeEquivalentTo(routes);

        await routeRepository
            .Received(1)
            .GetAll(true);
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_List_From_Cache()
    {
        var routes = new RouteBuilder().BuildList();

        var routeRepository = Substitute.For<IRouteRepository>();

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<IList<Route>>())
            .Returns(x =>
            {
                if ((string)x[0] == CacheKeys.RoutesKey)
                {
                    x[1] = routes;
                    return true;
                }

                return false;
            });

        var service = new ProviderDataServiceBuilder()
            .Build(routeRepository: routeRepository,
                cache: cache);

        var results = await service.GetRoutes();
        results.Should().BeEquivalentTo(routes);

        await routeRepository
            .DidNotReceive()
            .GetAll(true);
    }

    [Fact]
    public async Task GetAllProviders_Returns_Expected_List()
    {
        var providers = new ProviderDetailBuilder().BuildList().ToList();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.GetAll()
            .Returns(providers);

        var service = new ProviderDataServiceBuilder().Build(
            providerRepository: providerRepository);

        var response = await service.GetAllProviders();
        response.Should().NotBeNull();
        response.Providers.Should().BeEquivalentTo(providers);
    }

    [Fact]
    public async Task GetCsv_Returns_Non_Empty_Data()
    {
        var providers = new ProviderDetailFlatBuilder()
            .BuildList()
            .ToList();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.GetAllFlattened()
            .Returns(providers);

        var service = new ProviderDataServiceBuilder().Build(
            providerRepository: providerRepository);

        var response = await service.GetCsv();
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetCsv_Returns_Expected_Row_Header()
    {
        var providers = new ProviderDetailFlatBuilder().BuildList().ToList();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.GetAllFlattened()
            .Returns(providers);

        var service = new ProviderDataServiceBuilder().Build(
            providerRepository: providerRepository);

        var response = await service.GetCsv();

        using var stream = new MemoryStream(response);
        using var parser = new TextFieldParser(stream)
        {
            TextFieldType = FieldType.Delimited,
            Delimiters = new[] { "," }
        };

        parser.EndOfData.Should().BeFalse();
        var headerRow = parser.ReadFields();
        headerRow.Should().NotBeNull();

        headerRow.Should().BeEquivalentTo(
            "UKPRN",
            "Provider Name",
            "Postcode",
            "Location Name",
            "Address Line 1",
            "Address Line 2",
            "Town",
            "County",
            "Email",
            "Telephone",
            "Website",
            "Year of Delivery",
            "Route Name",
            "Qualification Name"
        );
    }

    [Fact]
    public async Task GetCsv_Returns_Expected_Provider_Data()
    {
        var providers = new ProviderDetailFlatBuilder()
            .BuildList()
            .ToList();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.GetAllFlattened()
            .Returns(providers);

        var service = new ProviderDataServiceBuilder().Build(
            providerRepository: providerRepository);

        var response = await service.GetCsv();

        using var stream = new MemoryStream(response);
        using var parser = new TextFieldParser(stream)
        {
            TextFieldType = FieldType.Delimited,
            Delimiters = new[] { "," }
        };

        parser.EndOfData.Should().BeFalse();
        parser.ReadFields(); //Skip header
        parser.EndOfData.Should().BeFalse();

        var index = 0;
        while (!parser.EndOfData)
        {
            var dataRow = parser.ReadFields();
            dataRow.Should().BeEquivalentTo(new[]
            {
                providers[index].UkPrn.ToString(),
                providers[index].ProviderName,
                providers[index].Postcode,
                providers[index].LocationName,
                providers[index].AddressLine1,
                providers[index].AddressLine2,
                providers[index].Town,
                providers[index].County,
                providers[index].Email,
                providers[index].Telephone,
                providers[index].Website,
                providers[index].Year.ToString(),
                providers[index].RouteName,
                providers[index].QualificationName
            }, $"data on providers row {index} should match output");
            index++;
        }

        index.Should().Be(providers.Count);
    }

    [Fact]
    public async Task FindProviders_Passes_All_Parameters()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((new ProviderSearchResultBuilder().BuildList(), totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(fromGeoLocation.Location)
            .Returns(fromGeoLocation);

        var searchSettings = new SettingsBuilder()
            .BuildSearchSettings();

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            searchSettings: searchSettings);

        var results = await service
            .FindProviders(fromGeoLocation.Location,
                _testRouteIds,
                _testQualificationIds,
                TestPage,
                TestPageSize);

        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().NotBeNullOrEmpty();
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .Received(1)
            .GetPostcode(fromGeoLocation.Location);

        await providerRepository
            .Received()
            .Search(Arg.Is<GeoLocation>(p =>
                    p.Location == fromGeoLocation.Location &&
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    p.Latitude == fromGeoLocation.Latitude &&
                    p.Longitude == fromGeoLocation.Longitude),
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    Arg.Is<IList<int>>(r => r.ListIsEquivalentTo(_testRouteIds)),
                Arg.Is<IList<int>>(q => q.ListIsEquivalentTo(_testQualificationIds)),
                Arg.Is<int>(p => p == TestPage),
                Arg.Is<int>(s => s == TestPageSize),
                Arg.Is<bool>(b => b == searchSettings.MergeAdditionalProviderData));
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(fromGeoLocation.Location)
            .Returns(fromGeoLocation);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository);

        var results = await service.FindProviders(fromGeoLocation.Location);
        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .Received(1)
            .GetPostcode(fromGeoLocation.Location);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_List_For_Valid_Postcode_From_Cache()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidPostcodeLocation();
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var cache = Substitute.For<IMemoryCache>();
        cache.TryGetValue(Arg.Any<string>(), out Arg.Any<GeoLocation>())
            .Returns(x =>
            {
                if (((string)x[0]).Contains(fromGeoLocation.Location.Replace(" ", "")))
                {
                    x[1] = GeoLocationBuilder.BuildGeoLocation(fromGeoLocation.Location);
                    return true;
                }

                return false;
            });

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            cache: cache);

        var results = await service.FindProviders(fromGeoLocation.Location);
        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().BeEquivalentTo(searchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_List_For_Valid_Outcode()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidOutwardPostcodeLocation();
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetOutcode(fromGeoLocation.Location)
            .Returns(fromGeoLocation);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository);

        var results = await service.FindProviders(fromGeoLocation.Location);
        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be(fromGeoLocation.Location);
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(fromGeoLocation.Location);
        await postcodeLookupService
            .Received(1)
            .GetOutcode(fromGeoLocation.Location);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Valid_Town_With_Dot()
    {
        const string searchTerms = "St. Agnes, Cornwall";
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;
        var towns = new TownBuilder().BuildListForStAgnes();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == searchTerms),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(Arg.Any<string>())
            .Returns(towns);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            townDataService: townDataService);

        var results = await service.FindProviders(searchTerms);

        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be("St. Agnes, Cornwall");
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
        await townDataService
            .Received(1)
            .Search(searchTerms);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Valid_Town_With_Dot_And_Partial_Search_Term()
    {
        const string searchTerms = "St. Agnes";
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;
        var towns = new TownBuilder().BuildListForStAgnes();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location.StartsWith(searchTerms)),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(Arg.Any<string>())
            .Returns(towns);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            townDataService: townDataService);

        var results = await service.FindProviders(searchTerms);

        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be("St. Agnes, Cornwall");
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
        await townDataService
            .Received(1)
            .Search(searchTerms);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Valid_Town_With_Dot_And_Partial_Search_Term_Lower_Case()
    {
        const string searchTerms = "st. agnes";
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;
        var towns = new TownBuilder().BuildListForStAgnes();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p =>
                    p.Location
                        .StartsWith(searchTerms, StringComparison.CurrentCultureIgnoreCase)),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(Arg.Any<string>())
            .Returns(towns);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            townDataService: townDataService);

        var results = await service.FindProviders(searchTerms);

        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be("St. Agnes, Cornwall");
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
        await townDataService
            .Received(1)
            .Search(searchTerms);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Valid_Town_Without_Dot()
    {
        const string searchTerms = "St Agnes, Cornwall";
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;
        var towns = new TownBuilder().BuildListForStAgnes();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == searchTerms),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((searchResults, totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(Arg.Any<string>())
            .Returns(towns);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            townDataService: townDataService);

        var results = await service.FindProviders(searchTerms);

        results.Should().NotBeNull();
        results.Error.Should().BeNull();
        results.SearchTerm.Should().Be("St Agnes, Cornwall");
        results.SearchResults.Should().BeEquivalentTo(searchResults);
        results.TotalResults.Should().Be(totalSearchResults);

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
        await townDataService
            .Received(1)
            .Search(searchTerms);
    }

    [Fact]
    public async Task FindProviders_Returns_Expected_Error_Details_For_Bad_Postcode()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildInvalidPostcodeLocation();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<bool>())
            .Returns((new ProviderSearchResultBuilder().BuildList(), totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.Search(Arg.Any<string>())
            .Returns(new List<Town>());

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository,
            townDataService: townDataService);

        var results = await service.FindProviders(fromGeoLocation.Location);
        results.Should().NotBeNull();
        results.Error.Should().Be("The postcode was not found");
        results.SearchTerm.Should().BeNull();
        results.SearchResults.Should().BeNull();
        results.TotalResults.Should().BeNull();

        await postcodeLookupService
            .DidNotReceive()
            .GetPostcode(Arg.Any<string>());
        await townDataService
            .Received(1)
            .Search(fromGeoLocation.Location);
    }

    [Fact]
    public async Task HasQualifications_Calls_Repository()
    {
        var qualificationRepository = Substitute.For<IQualificationRepository>();
        qualificationRepository.HasAny()
            .Returns(true);

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository);

        var result = await service.HasQualifications();

        result.Should().BeTrue();

        await qualificationRepository
            .Received(1)
            .HasAny();
    }

    [Fact]
    public async Task HasProviders_Calls_Repository()
    {
        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.HasAny()
            .Returns(true);

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        var result = await service.HasProviders();

        result.Should().BeTrue();

        await providerRepository
            .Received(1)
            .HasAny();
    }
    
    [Fact]
    public async Task ImportProviderData_Calls_Repository_To_Save_Data()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        await using var stream = ProviderDataJsonBuilder.BuildProviderDataStream();

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.ImportProviderData(stream, true);

        await providerRepository
            .Received(1)
            .Save(Arg.Any<IList<Application.Models.Provider>>(),
                Arg.Is<bool>(b => b));
    }

    [Fact]
    public async Task ImportProviderData_Calls_Repository_To_Save_Data_With_Additional_Data_Flag_Set()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        IList<Application.Models.Provider> receivedProviders = null;

        await providerRepository
            .Save(Arg.Do<IList<Application.Models.Provider>>(
                x => receivedProviders = x),
                Arg.Any<bool>());

        await using var stream = ProviderDataJsonBuilder.BuildProviderDataStream();

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.ImportProviderData(stream, true);

        receivedProviders.Should().NotBeNullOrEmpty();

        foreach (var provider in receivedProviders)
        {
            provider.IsAdditionalData.Should().BeTrue();
            foreach (var location in provider.Locations)
            {
                location.IsAdditionalData.Should().BeTrue();
            }
        }
    }

    [Fact]
    public async Task ImportProviderData_Loads_Expected_Provider()
    {
        var providerRepository = Substitute.For<IProviderRepository>();

        IList<Application.Models.Provider> receivedProviders = null;

        await providerRepository
            .Save(Arg.Do<IList<Application.Models.Provider>>(
                x => receivedProviders = x),
                Arg.Any<bool>());

        await using var stream = ProviderDataJsonBuilder.BuildProviderDataStream();

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await service.ImportProviderData(stream, true);

        receivedProviders.Should().NotBeNullOrEmpty();

        var provider = receivedProviders
            .SingleOrDefault(p =>
                p.UkPrn == TestUkPrn);
        provider.Should().NotBeNull();

        // ReSharper disable once StringLiteralTypo
        provider.Validate(
            TestUkPrn,
        "TEST SCHOOL",
            null,
            null,
            "London",
            null,
            "SW17 0AQ",
        "info@testschool.com",
        "020 5555 5555",
            "https://www.testschool.com/",
            locationCount: 1,
            isAdditionalData: true);

        var location = provider!.Locations.First();

        // ReSharper disable once StringLiteralTypo
        location.Validate(
            "TEST SCHOOL",
            "SW17 0AQ",
            null,
            null,
            "London",
            null,
            "info@testschool.com",
            "020 5555 5555",
            "https://www.testschool.com/",
            51.439999,
            -0.1789765,
            1);
        var deliveryYear = location.DeliveryYears.First();

        deliveryYear.Validate(2022,
            new[] { 38 });
    }

    [Fact]
    public async Task ImportProviderData_Clears_Caches()
    {
        var cache = Substitute.For<IMemoryCache>();

        await using var stream = ProviderDataJsonBuilder.BuildProviderDataStream();

        var service = new ProviderDataServiceBuilder()
            .Build(cache: cache);

        await service.ImportProviderData(stream, true);
        
        cache
            .Received(1)
            .Remove(CacheKeys.QualificationsKey);

        cache
            .Received(1)
            .Remove(CacheKeys.ProviderDataDownloadInfoKey);

        cache
            .Received(1)
            .Remove(CacheKeys.RoutesKey);
    }

    [Fact]
    public async Task ImportProviderContacts_Works_As_Expected()
    {
        var receivedProviders = new List<ProviderContactDto>();

        var providerRepository = Substitute.For<IProviderRepository>();
        await providerRepository
            .UpdateProviderContacts(Arg.Do<IEnumerable<ProviderContactDto>>(
                p =>
                    receivedProviders.AddRange(p)));

        var service = new ProviderDataServiceBuilder()
            .Build(providerRepository: providerRepository);

        await using var stream = ProviderContactsCsvBuilder
            .BuildProviderContactsCsvAsStream();

        await service.ImportProviderContacts(stream);

        await providerRepository
            .Received(1)
            .UpdateProviderContacts(Arg.Is<IEnumerable<ProviderContactDto>>(
                p => p.Any()));

        var providerContacts = receivedProviders.SingleOrDefault(p => p.UkPrn == 10000055);

        var expectedContacts = new ProviderContactDtoBuilder().Build();
        providerContacts.Validate(
            new ProviderContactDto
            {
                UkPrn = 10000055,
                Name = "ABINGDON AND WITNEY COLLEGE",
                EmployerContactEmail = "employer.guidance@abingdon-witney.ac.uk",
                EmployerContactTelephone = "01235 789010",
                EmployerContactWebsite = "http://www.abingdon-witney.ac.uk/employers",
                StudentContactEmail = "student.counseller@abingdon-witney.ac.uk",
                StudentContactTelephone = "01235 789010",
                StudentContactWebsite = "http://www.abingdon-witney.ac.uk/students"
            });

        providerContacts.Validate(expectedContacts);

    }
}