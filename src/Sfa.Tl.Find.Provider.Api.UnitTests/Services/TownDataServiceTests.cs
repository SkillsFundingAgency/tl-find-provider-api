using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services;

public class TownDataServiceTests
{
    private const string BaseUriString = "https://services1.arcgis.com/ESMARspQHYMw9BZ9/arcgis/rest/services/IPN_GB_2016/FeatureServer/0/query?where=ctry15nm%20%3D%20'ENGLAND'%20AND%20popcnt%20%3E%3D%20500%20AND%20popcnt%20%3C%3D%2010000000&outFields=placeid,place15nm,ctry15nm,cty15nm,ctyltnm,lat,long&returnDistinctValues=true&outSR=4326&f=json";
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
            .Build(
                responses,
                townRepository: townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(2);

        // ReSharper disable StringLiteralTypo
        receivedTowns.Should().Contain(t => t.Name == "Abbas and Templecombe" &&
                                            t.County == "Somerset" &&
                                            t.LocalAuthorityName == "Somerset" &&
                                            t.Latitude == 51.002593M &&
                                            t.Longitude == -2.411563M);

        receivedTowns.Should().Contain(t => t.Name == "Abberley" &&
                                            t.County == "Worcestershire" &&
                                            t.LocalAuthorityName == "Worcestershire" &&
                                            t.Latitude == 52.302702M &&
                                            t.Longitude == -2.391708M);
        // ReSharper restore StringLiteralTypo
    }
}