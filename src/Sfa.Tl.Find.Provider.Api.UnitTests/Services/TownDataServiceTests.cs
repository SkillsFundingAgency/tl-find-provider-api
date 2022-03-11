using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
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
            .Build(townRepository);

        var result = await service.HasTowns();

        result.Should().BeTrue();

        await townRepository
            .Received(1)
            .HasAny();
    }

    [Fact]
    public async Task ImportTowns_Creates_Expected_Towns()
    {
        //var responses = new Dictionary<string, string>
        //{
        //    { CourseDirectoryService.QualificationsEndpoint, CourseDirectoryJsonBuilder.BuildValidTLevelDefinitionsResponse() }
        //};

        IList<Town> receivedTowns = null;

        var townRepository = Substitute.For<ITownRepository>();
        await townRepository
            .Save(Arg.Do<IEnumerable<Town>>(
                x => receivedTowns = x?.ToList()));

        var service = new TownDataServiceBuilder()
            .Build(
                //responses, 
                townRepository: townRepository);

        await service.ImportTowns();

        receivedTowns.Should().NotBeNull();
        receivedTowns.Count.Should().Be(5);

        //receivedTowns.Should().Contain(q => q.Id == 36 && q.Name == "Design, Surveying and Planning for Construction");
    }
}