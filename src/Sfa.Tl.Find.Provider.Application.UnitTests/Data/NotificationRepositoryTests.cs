using System.Data;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
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
}