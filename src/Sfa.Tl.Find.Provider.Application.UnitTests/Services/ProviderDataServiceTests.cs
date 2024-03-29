﻿using Microsoft.VisualBasic.FileIO;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class ProviderDataServiceTests
{
    private const int TestPage = 3;
    private const int TestPageSize = Constants.DefaultPageSize + 10;

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

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Industry>>(CacheKeys.IndustriesKey)
            .Returns(default(IList<Industry>));

        var service = new ProviderDataServiceBuilder()
            .Build(industryRepository: industryRepository,
                cacheService: cacheService);

        var results = (await service.GetIndustries()).ToList();
        results.Should().BeEquivalentTo(industries);

        await industryRepository
            .Received(1)
            .GetAll();
    }

    [Fact]
    public async Task GetIndustries_Returns_Expected_List_From_Cache()
    {
        var industries = new IndustryBuilder().BuildList().ToList();

        var industryRepository = Substitute.For<IIndustryRepository>();

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Industry>>(CacheKeys.IndustriesKey)
            .Returns(industries);

        var service = new ProviderDataServiceBuilder()
            .Build(industryRepository: industryRepository,
                cacheService: cacheService);

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

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Qualification>>(CacheKeys.QualificationsKey)
            .Returns(default(IList<Qualification>));

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository,
                cacheService: cacheService);

        var results = (await service.GetQualifications()).ToList();
        results.Should().BeEquivalentTo(qualifications);

        await qualificationRepository
            .Received(1)
            .GetAll();
    }

    [Fact]
    public async Task GetQualifications_Returns_Expected_List_From_Cache()
    {
        var qualifications = new QualificationBuilder().BuildList().ToList();

        var qualificationRepository = Substitute.For<IQualificationRepository>();

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Qualification>>(CacheKeys.QualificationsKey)
            .Returns(qualifications);

        var service = new ProviderDataServiceBuilder()
            .Build(qualificationRepository: qualificationRepository,
                cacheService: cacheService);

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
        routeRepository.GetAll()
            .Returns(routes);

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Route>>(CacheKeys.RoutesKey)
            .Returns(default(IList<Route>));

        var service = new ProviderDataServiceBuilder()
            .Build(routeRepository: routeRepository,
                cacheService: cacheService);

        var results = (await service.GetRoutes()).ToList();
        results.Should().BeEquivalentTo(routes);

        await routeRepository
            .Received(1)
            .GetAll();
    }

    [Fact]
    public async Task GetRoutes_Returns_Expected_List_From_Cache()
    {
        var routes = new RouteBuilder().BuildList().ToList();

        var routeRepository = Substitute.For<IRouteRepository>();

        var cacheService = Substitute.For<ICacheService>();
        cacheService.Get<IList<Route>>(CacheKeys.RoutesKey)
            .Returns(routes);

        var service = new ProviderDataServiceBuilder()
            .Build(routeRepository: routeRepository,
                cacheService: cacheService);

        var results = await service.GetRoutes();
        results.Should().BeEquivalentTo(routes);

        await routeRepository
            .DidNotReceive()
            .GetAll();
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
                Arg.Any<int>())
            .Returns((new ProviderSearchResultBuilder().BuildList(), totalSearchResults));

        var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
        postcodeLookupService.GetPostcode(fromGeoLocation.Location)
            .Returns(fromGeoLocation);

        var service = new ProviderDataServiceBuilder().Build(
            postcodeLookupService: postcodeLookupService,
            providerRepository: providerRepository);

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
                    Arg.Is<int>(s => s == TestPageSize));
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
                Arg.Any<int>())
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
    public async Task FindProviders_Returns_Expected_List_For_Valid_Outcode()
    {
        var fromGeoLocation = GeoLocationBuilder.BuildValidOutcodeLocation();
        var searchResults = new ProviderSearchResultBuilder().BuildList().ToList();
        const int totalSearchResults = 10;

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.Search(
                Arg.Is<GeoLocation>(p => p.Location == fromGeoLocation.Location),
                Arg.Any<List<int>>(),
                Arg.Any<List<int>>(),
                Arg.Any<int>(),
                Arg.Any<int>())
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
                Arg.Any<int>())
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
                Arg.Any<int>())
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
    public async Task
        FindProviders_Returns_Expected_Error_Details_For_Valid_Town_With_Dot_And_Partial_Search_Term_Lower_Case()
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
                Arg.Any<int>())
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
                Arg.Any<int>())
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
                Arg.Any<int>())
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
    public async Task GetLocationPostcodes_Returns_Expected_List()
    {
        const long ukPrn = 12345678;

        var locationPostcodes = new LocationPostcodeBuilder()
            .BuildList()
            .ToList();

        var providerRepository = Substitute.For<IProviderRepository>();
        providerRepository.GetLocationPostcodes(ukPrn)
            .Returns(locationPostcodes);

        var service = new ProviderDataServiceBuilder().Build(
            providerRepository: providerRepository);

        var response = (await service
            .GetLocationPostcodes(ukPrn))
            ?.ToList();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(locationPostcodes);
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
}