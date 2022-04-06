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
    private const string BaseUriString = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lad15nm,laddescnm,lat,long,descnm&returnDistinctValues=true&outSR=4326&f=json";
    private const string FirstPageUriString = $"{BaseUriString}&resultRecordCount=2000&resultOffSet=0";
    private const string ThirdPageUriString = $"{BaseUriString}&resultRecordCount=999&resultOffSet=2";

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
        receivedTowns.Count.Should().Be(4);

        // ReSharper disable StringLiteralTypo
        // ReSharper disable CommentTypo
        ValidateTown(receivedTowns.SingleOrDefault(t => t.Id == 2),
            2,
            "Abbas and Templecombe",
            "Somerset",
            "Somerset",
            //"South Somerset",
            //"NMD",
            51.002593M,
            -2.411563M);

        ValidateTown(receivedTowns.SingleOrDefault(t => t.Id == 4),
            4,
            "Abberley",
            "Worcestershire",
            "Worcestershire",
            //"Malvern Hills",
            //"NMD",
            52.302702M,
            -2.391708M);
        // ReSharper restore StringLiteralTypo
        // ReSharper restore CommentTypo
    }

    [Fact]
    public async Task ImportTowns_DeDuplicates_Towns()
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

        var abingdon = receivedTowns.Where(t => t.Name == "Abingdon");
        var abingdonInOxfordshire = receivedTowns
            .Where(t => t.Name == "Abingdon" && t.County == "Oxfordshire")
            .ToList();

        abingdon.Count().Should().Be(2);
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