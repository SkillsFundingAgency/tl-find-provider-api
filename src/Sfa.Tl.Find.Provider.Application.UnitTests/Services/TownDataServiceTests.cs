using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using System.IO;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Csv;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class TownDataServiceTests
{
    private const string FirstPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=2000&resultOffSet=0";
    private const string ThirdPageUriString = $"{TownDataService.OfficeForNationalStatisticsLocationUrl}&resultRecordCount=999&resultOffSet=2";

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(TownDataService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
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
        var uri = TownDataService.GetUri(2, 999);

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
        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 302)
            .Validate(302,
                "Abingdon",
                "Oxfordshire",
                "Oxfordshire",
                51.674302M,
                -1.282302M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 304)
            .Validate(304,
                "Abingdon",
                "Inner London",
                "Greater London",
                51.497681M,
                -0.192782M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72832)
            .Validate(
                72832,
                "West Bromwich",
                "West Midlands",
                "West Midlands",
                52.530629M,
                -2.005941M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72834)
            .Validate(
                72834,
                "West Bromwich (East)",
                "West Midlands",
                "West Midlands",
                52.540693M,
                -1.942085M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72835)
            .Validate(
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

        abingdonInOxfordshire
            .Single()
            .Validate(302,
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

        westBromwich
            .Single()
            .Validate(72832,
                "West Bromwich",
                "West Midlands",
                "West Midlands",
                //"Sandwell",
                //"MD",
                52.530629M,
                -2.005941M);
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Creates_Expected_Number_Of_Towns()
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
    public async Task ImportTowns_From_Csv_Stream_Creates_Expected_Towns()
    {
        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = IndexOfPlaceNamesCsvBuilder.BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);

        // ReSharper disable StringLiteralTypo
        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 302)
            .Validate(302,
                "Abingdon",
                "Oxfordshire",
                "Oxfordshire",
                51.674302M,
                -1.282302M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 304)
            .Validate(304,
                "Abingdon",
                "Inner London",
                "Greater London",
                51.497681M,
                -0.192782M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72832)
            .Validate(
                72832,
                "West Bromwich",
                "West Midlands",
                "West Midlands",
                52.530629M,
                -2.005941M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72834)
            .Validate(
                72834,
                "West Bromwich (East)",
                "West Midlands",
                "West Midlands",
                52.540693M,
                -1.942085M);

        receivedTowns
            .SingleOrDefault(t =>
                t.Id == 72835)
            .Validate(
                72835,
                "West Bromwich Central",
                "West Midlands",
                "West Midlands",
                52.520416M,
                -1.984158M);
        // ReSharper restore StringLiteralTypo
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_From_Csv_Stream_Filters_Out_Civil_Parishes()
    {
        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = IndexOfPlaceNamesCsvBuilder.BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        await service.ImportTowns(stream);

        // ReSharper disable StringLiteralTypo
        receivedTowns.Should().NotContain(t =>
                t.Name == "Abbas and Templecombe" ||
                t.Name == "Abberley");
        // ReSharper restore StringLiteralTypo
    }


    [Fact]
    public async Task ImportTowns_From_Csv_Stream_From_Csv_Stream_Filters_Out_Non_English_Towns()
    {
        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = IndexOfPlaceNamesCsvBuilder.BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        await service.ImportTowns(stream);

        // ReSharper disable once StringLiteralTypo
        receivedTowns.Should().NotContain(t => t.Name == "Aberdovey");
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Deduplicates_Abingdon_Correctly()
    {
        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = IndexOfPlaceNamesCsvBuilder.BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();

        var abingdonInstances
            = receivedTowns.Where(t => t.Name == "Abingdon");
        var abingdonInOxfordshire = receivedTowns
            .Where(t => t.Name == "Abingdon" && t.County == "Oxfordshire")
            .ToList();

        abingdonInstances.Count().Should().Be(2);
        abingdonInOxfordshire.Count.Should().Be(1);

        abingdonInOxfordshire
            .Single()
            .Validate(302,
                "Abingdon",
                "Oxfordshire",
                "Oxfordshire",
                //"Vale of White Horse",
                //"NMD",
                51.674302M,
                -1.282302M);
    }

    [Fact]
    public async Task ImportTowns_From_Csv_Stream_Deduplicates_WestBromwich_Correctly()
    {
        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var stream = IndexOfPlaceNamesCsvBuilder.BuildIndexOfPlaceNamesCsvAsStream();

        var service = new TownDataServiceBuilder()
            .Build(townRepository: townRepository);

        await service.ImportTowns(stream);

        receivedTowns.Should().NotBeNull();

        var westBromwich = receivedTowns
            .Where(t => t.Name == "West Bromwich")
            .ToList();

        westBromwich.Count.Should().Be(1);

        westBromwich
            .Single()
            .Validate(72832,
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
}