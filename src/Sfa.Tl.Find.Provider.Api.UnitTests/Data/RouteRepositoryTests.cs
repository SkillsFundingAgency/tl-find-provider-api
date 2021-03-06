using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data;

public class RouteRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RouteRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }
        
    [Fact]
    public async Task GetAll_Returns_Expected_List()
    {
        var routes = new RouteBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var routeDtoList =
            routes.Select(r => new RouteDto
            {
                RouteId = r.Id,
                RouteName = r.Name
            }).ToList();

        var qualificationDtoList =
            routes.Select(r => new QualificationDto
            {
                QualificationId = r.Id,
                QualificationName = r.Name,
                NumberOfQualificationsOffered = r.Qualifications?.Count ?? 0
            }).ToList();

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
            .GetAll(true))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results[0].Id.Should().Be(routeDtoList[0].RouteId);
        results[0].Name.Should().Be(routeDtoList[0].RouteName);
        results[0].NumberOfQualifications.Should().Be(1);
        results[0].NumberOfQualificationsOffered.Should().Be(1);
        results[0].Qualifications.Should().NotBeNullOrEmpty();
        results[0].Qualifications[0].Id.Should().Be(qualificationDtoList[0].QualificationId);
        results[0].Qualifications[0].Name.Should().Be(qualificationDtoList[0].QualificationName);

        await dbContextWrapper
            .Received(1)
            .QueryAsync(dbConnection,
                "GetRoutes",
                Arg.Any<Func<RouteDto, QualificationDto, Route>>(),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure);
    }
}