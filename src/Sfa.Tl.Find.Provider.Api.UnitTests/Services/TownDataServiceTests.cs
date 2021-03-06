using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services;

public class TownDataServiceTests
{
    private const string FirstPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=2000&resultOffSet=0";
    private const string ThirdPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=999&resultOffSet=2";

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(TownDataService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(TownDataService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task HasTowns_Calls_Repository()
    {
        var townRepository = Substitute.For<ITownRepository>();
        townRepository.HasAny()
            .Returns(true);

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        var result = await service.HasTowns();

        result.Should().BeTrue();

        await townRepository
            .Received(1)
            .HasAny();
    }

    [Fact]
    public void GetUri_Returns_Expected_Value()
    {
        var service = new TownDataServiceBuilder()
                .Build();
        var uri = service.GetUri(2, 999);

        uri.Should().NotBeNull();
        uri.AbsoluteUri.Should().Be(ThirdPageUriString);
    }

    [Fact]
    public async Task ImportTowns_Creates_Expected_Number_Of_Towns()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, NationalStatisticsJsonBuilder.BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);
    }

    [Fact]
    public async Task ImportTowns_Creates_Expected_Towns()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, NationalStatisticsJsonBuilder.BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);

        // ReSharper disable StringLiteralTypo
        ValidateTown(receivedTowns
                .SingleOrDefault(t => 
                    t.Id == 302),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            51.674302M,
            -1.282302M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t => 
                    t.Id == 304),
            304,
            "Abingdon",
            "Inner London",
            "Greater London",
            51.497681M,
            -0.192782M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72832),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            52.530629M,
            -2.005941M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72834),
            72834,
            "West Bromwich (East)",
            "West Midlands",
            "West Midlands",
            52.540693M,
            -1.942085M);

        ValidateTown(receivedTowns
                .SingleOrDefault(t =>
                    t.Id == 72835),
            72835,
            "West Bromwich Central",
            "West Midlands",
            "West Midlands",
            52.520416M,
            -1.984158M);
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_Filters_Out_Civil_Parishes()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, NationalStatisticsJsonBuilder.BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                townRepository);

        await service.ImportTowns();

        // ReSharper disable StringLiteralTypo
        receivedTowns.Should().NotContain(t => 
                t.Name == "Abbas and Templecombe" ||
                t.Name == "Abberley");
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_Deduplicates_Abingdon_Correctly()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, NationalStatisticsJsonBuilder.BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();

        var abingdonInstances
            = receivedTowns.Where(t => t.Name == "Abingdon");
        var abingdonInOxfordshire = receivedTowns
            .Where(t => t.Name == "Abingdon" && t.County == "Oxfordshire")
            .ToList();

        abingdonInstances.Count().Should().Be(2);
        abingdonInOxfordshire.Count.Should().Be(1);

        ValidateTown(abingdonInOxfordshire.Single(),
            302,
            "Abingdon",
            "Oxfordshire",
            "Oxfordshire",
            //"Vale of White Horse",
            //"NMD",
            51.674302M,
            -1.282302M);
    }

    [Fact]
    public async Task ImportTowns_Deduplicates_WestBromwich_Correctly()
    {
        var responses = new Dictionary<string, string>
        {
            { FirstPageUriString, NationalStatisticsJsonBuilder.BuildNationalStatisticsLocationsResponse() }
        };

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(responses,
                townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();

        var westBromwich = receivedTowns
            .Where(t => t.Name == "West Bromwich")
            .ToList();

        westBromwich.Count.Should().Be(1);

        ValidateTown(westBromwich.Single(),
            72832,
            "West Bromwich",
            "West Midlands",
            "West Midlands",
            //"Sandwell",
            //"MD",
            52.530629M,
            -2.005941M);
    }
    
    [Fact]
    public async Task Search_Calls_Repository()
    {
        const string searchTerm = "Coventry";

        var towns = new TownBuilder()
            .BuildList()
            .ToList();

        var townRepository = Substitute.For<ITownRepository>();
        townRepository.Search(searchTerm, Constants.TownSearchDefaultMaxResults)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        var result = await service
            .Search(searchTerm);

        result.Should().BeEquivalentTo(towns);

        await townRepository
            .Received(1)
            .Search(searchTerm, Constants.TownSearchDefaultMaxResults);
    }

    [Fact]
    public async Task Search_Calls_Repository_With_Max_Results()
    {
        const string searchTerm = "Coventry";
        const int maxResults = 10;

        var towns = new TownBuilder()
            .BuildList()
            .ToList();

        var townRepository = Substitute.For<ITownRepository>();
        townRepository.Search(searchTerm, maxResults)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        var result = await service
            .Search(searchTerm, maxResults);

        result.Should().BeEquivalentTo(towns);

        await townRepository
            .Received(1)
            .Search(searchTerm, maxResults);
    }

    [Fact]
    public async Task Search_Does_Not_Call_Repository_For_Postcode()
    {
        const string searchTerm = "CV1 2WT";

        var towns = new TownBuilder()
            .BuildList()
            .ToList();

        var townRepository = Substitute.For<ITownRepository>();
        townRepository.Search(searchTerm, Constants.TownSearchDefaultMaxResults)
            .Returns(towns);

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        var result = await service
            .Search(searchTerm);

        result.Should().BeEmpty();

        await townRepository
            .DidNotReceive()
            .Search(Arg.Any<string>(), Arg.Any<int>());
    }

    private static void ValidateTown(Town town, 
        int id, 
        string name, 
        string county, 
        string localAuthority,
        decimal latitude, 
        decimal longitude)
    {
        town.Should().NotBeNull();
        town.Id.Should().Be(id);
        town.Name.Should().Be(name);
        town.County.Should().Be(county);
        town.LocalAuthority.Should().Be(localAuthority);
        town.Latitude.Should().Be(latitude);
        town.Longitude.Should().Be(longitude);
    }
}