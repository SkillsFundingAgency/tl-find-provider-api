using System.Data;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class RouteRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(RouteRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();
        var qualificationDtoList = new QualificationDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetRoutes",
                Arg.Do<Func<RouteDto, QualificationDto, Route>>(
                    x =>
                    {
                        var r = routeDtoList[callIndex];
                        var q = qualificationDtoList[callIndex];
                        x.Invoke(r, q);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new RouteRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository
            .GetAll())
            .ToList();

        results.Count.Should().Be(1);
        results[0].Validate(routeDtoList[0]);

        results[0].NumberOfQualifications.Should().Be(1);
        results[0].NumberOfQualificationsOffered.Should().Be(1);

        results[0].Qualifications.Should().NotBeNullOrEmpty();
        results[0].Qualifications[0].Validate(qualificationDtoList[0]);
    }
}