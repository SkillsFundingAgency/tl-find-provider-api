using System.Data;
using Dapper;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Data;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Data;

public class NotificationRepositoryTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(NotificationRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetNotifications_Returns_Expected_Results()
    {
        const long ukPrn = 12345678;
        const bool includeAdditionalData = true;

        var notifications = new NotificationBuilder()
            .BuildList()
            .ToList();
        var notificationDtoList = new NotificationDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Notification>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(notifications);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetNotifications",
                Arg.Do<Func<NotificationDto, RouteDto, Notification>>(
                    x =>
                    {
                        var e = notificationDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetNotifications(ukPrn, includeAdditionalData))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(notificationDtoList.First());
        results[0].Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        results[0].Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }


    [Fact]
    public async Task GetNotification_Returns_Expected_Results()
    {
        const int id = 1;

        var notifications = new NotificationBuilder()
            .BuildList()
            .ToList();
        var notificationDtoList = new NotificationDtoBuilder()
            .BuildList()
            .Take(1)
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<Notification>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(notifications);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetNotificationDetail",
                Arg.Do<Func<NotificationDto, RouteDto, Notification>>(
                    x =>
                    {
                        var e = notificationDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(e, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.GetNotification(id);

        result.Validate(notificationDtoList.First());
        result.Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        result.Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task SaveNotification_Calls_Database()
    {
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Save(notification);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "CreateOrUpdateNotification"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task SaveNotification_Sets_Dynamic_Parameters()
    {
        var notification = new NotificationBuilder()
            .Build();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Save(notification);



        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();
        templates.Should().NotBeNullOrEmpty();

        var item = templates!.First();
        var pi = item.GetType().GetProperties();

        var expectedRouteIds = notification.Routes
                            .Select(r => r.Id);

        templates.GetDynamicTemplatesCount().Should().Be(3);
        templates.ContainsNameAndValue("locationId", notification.LocationId);
        templates.ContainsNameAndValue("searchRadius", notification.SearchRadius);
        var routeIds = templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds");
        routeIds.Should().NotBeNull();
        var t = routeIds.GetType().Name;
    }
}