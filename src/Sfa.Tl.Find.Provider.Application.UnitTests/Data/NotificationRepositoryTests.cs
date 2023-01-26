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
    private const int TestUkPrn = 10099099;

    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(NotificationRepository)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task GetNotificationSummaryList_Returns_Expected_Results()
    {
        const bool includeAdditionalData = true;

        var notificationSummaries = new NotificationSummaryBuilder()
            .BuildList()
            .ToList();
        var notificationSummaryDtoList = new NotificationSummaryDtoBuilder()
            .BuildList()
            .ToList();
        var locationDtoList = new LocationPostcodeDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<NotificationSummary>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(notificationSummaries);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetNotificationSummary",
                Arg.Do<Func<NotificationSummaryDto, LocationPostcodeDto, NotificationSummary>>(
                    x =>
                    {
                        var n = notificationSummaryDtoList[callIndex];
                        var l = locationDtoList[callIndex];
                        x.Invoke(n, l);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetNotificationSummaryList(TestUkPrn, includeAdditionalData))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(notificationSummaryDtoList.First());
        results[0].Locations.First().Validate(locationDtoList.First());
    }

    [Fact]
    public async Task GetNotificationSummaryList_Sets_Dynamic_Parameters()
    {
        const bool includeAdditionalData = true;

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetNotificationSummaryList(TestUkPrn, includeAdditionalData);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(2);
        templates.ContainsNameAndValue("ukPrn", TestUkPrn);
        templates.ContainsNameAndValue("includeAdditionalData", includeAdditionalData);
    }

    [Fact]
    public async Task GetNotificationLocationSummaryList_Returns_Expected_Results()
    {
        const int notificationId = 1;

        var notificationSummaries = new NotificationLocationSummaryBuilder()
            .BuildList()
            .ToList();
        var notificationLocationSummaryDtoList = new NotificationLocationSummaryDtoBuilder()
            .BuildList()
            .ToList();
        var locationDtoList = new LocationPostcodeDtoBuilder()
            .BuildList()
            .ToList();
        var routeDtoList = new RouteDtoBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<NotificationLocationSummary>(dbConnection, Arg.Any<string>(), Arg.Any<object>())
            .Returns(notificationSummaries);

        var callIndex = 0;

        await dbContextWrapper
            .QueryAsync(dbConnection,
                "GetNotificationLocationSummary",
                Arg.Do<Func<NotificationLocationSummaryDto, LocationPostcodeDto, RouteDto, NotificationLocationSummary>>(
                    x =>
                    {
                        var n = notificationLocationSummaryDtoList[callIndex];
                        var l = locationDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(n, l, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: Arg.Any<string>(),
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetNotificationLocationSummaryList(notificationId))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(notificationLocationSummaryDtoList.First());
       
        results[0].Locations.Should().NotBeNullOrEmpty();
        results[0].Locations.First().Validate(locationDtoList.First());

        results[0].Routes.Should().NotBeNullOrEmpty();
        results[0].Routes.First().Validate(routeDtoList[0]);
    }

    [Fact]
    public async Task GetNotificationLocationSummaryList_Sets_Dynamic_Parameters()
    {
        const int notificationId = 1;

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetNotificationLocationSummaryList(notificationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("notificationId", notificationId);
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
                        var n = notificationDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(n, r);

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
    public async Task GetNotification_Sets_Dynamic_Parameters()
    {
        const int notificationId = 1;
        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetNotification(notificationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("notificationId", notificationId);
    }

    [Fact]
    public async Task CreateNotification_Calls_Database()
    {
        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Create(notification, TestUkPrn);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "CreateNotification"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task CreateNotification_Sets_Dynamic_Parameters()
    {
        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Create(notification, TestUkPrn);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(7);
        templates.ContainsNameAndValue("ukPrn", TestUkPrn);
        templates.ContainsNameAndValue("email", notification.Email);
        templates.ContainsNameAndValue("verificationToken", notification.EmailVerificationToken);
        templates.ContainsNameAndValue("frequency", notification.Frequency);
        templates.ContainsNameAndValue("locationId", notification.LocationId);
        templates.ContainsNameAndValue("searchRadius", notification.SearchRadius);
        templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds")
            .Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateNotification_Calls_Database()
    {
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Update(notification);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "UpdateNotification"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task UpdateNotification_Sets_Dynamic_Parameters()
    {
        var notification = new NotificationBuilder()
            .Build();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Update(notification);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(6);
        templates.ContainsNameAndValue("id", notification.Id);
        templates.ContainsNameAndValue("email", notification.Email);
        templates.ContainsNameAndValue("frequency", notification.Frequency);
        templates.ContainsNameAndValue("locationId", notification.LocationId);
        templates.ContainsNameAndValue("searchRadius", notification.SearchRadius);
        templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds")
            .Should().NotBeNull();
    }

    [Fact]
    public async Task SaveEmailVerificationToken_Calls_Database()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");
        var notification = new NotificationBuilder()
            .Build();
        
        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .Build();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        await repository.SaveEmailVerificationToken(notification.Id!.Value, notification.Email, verificationToken);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("UPDATE dbo.NotificationEmail") &&
                    s.Contains("SET VerificationToken = @verificationToken,") &&
                    s.Contains("ModifiedOn = GETUTCDATE()") &&
                    s.Contains("WHERE NotificationId = @notificationId") &&
                    s.Contains("AND Email = @email")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters));
    }

    [Fact]
    public async Task SaveEmailVerificationToken_Sets_Dynamic_Parameters()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");
        var notification = new NotificationBuilder()
            .Build();

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.SaveEmailVerificationToken(notification.Id!.Value, notification.Email, verificationToken);
        
        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(3);
        templates.ContainsNameAndValue("notificationId", notification.Id);
        templates.ContainsNameAndValue("email", notification.Email);
        templates.ContainsNameAndValue("verificationToken", verificationToken);
    }


    [Fact]
    public async Task RemoveEmailVerificationToken_Calls_Database()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .Build();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        await repository.RemoveEmailVerificationToken(verificationToken);
        
        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("UPDATE dbo.NotificationEmail") &&
                    s.Contains("SET VerificationToken = NULL") &&
                    s.Contains("ModifiedOn = GETUTCDATE()") &&
                    s.Contains("WHERE VerificationToken = @verificationToken")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters));
    }

    [Fact]
    public async Task RemoveEmailVerificationToken_Sets_Dynamic_Parameters()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");

        var dbContextWrapper = new DbContextWrapperBuilder()
            .BuildSubstitute();

        var dynamicParametersWrapper = new SubstituteDynamicParameterWrapper();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.RemoveEmailVerificationToken(verificationToken);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("verificationToken", verificationToken);
    }
}