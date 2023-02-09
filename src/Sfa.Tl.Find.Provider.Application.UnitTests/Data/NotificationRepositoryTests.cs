using System.Data;
using Dapper;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Repositories;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
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
    public async Task Delete_Calls_Database()
    {
        const int providerNotificationId = 1;

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper, dynamicParametersWrapper.DapperParameterFactory);

        await repository.Delete(providerNotificationId);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "DeleteProviderNotification"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task Delete_Sets_Dynamic_Parameters()
    {
        const int providerNotificationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) = 
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.Delete(providerNotificationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("providerNotificationId", providerNotificationId);
    }

    [Fact]
    public async Task DeleteLocation_Calls_Database()
    {
        const int notificationLocationId = 1;

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper, dynamicParametersWrapper.DapperParameterFactory);

        await repository.DeleteLocation(notificationLocationId);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "DeleteNotificationLocation"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task DeleteLocation_Sets_Dynamic_Parameters()
    {
        const int notificationLocationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.DeleteLocation(notificationLocationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("notificationLocationId", notificationLocationId);
    }

    [Fact]
    public async Task GetAvailableLocationPostcodes_Returns_Expected_List_For_Single_Result_Row()
    {
        const int providerNotificationId = 1;

        var locationNames = new NotificationLocationNameBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<NotificationLocationName>(dbConnection,
                "GetProviderNotificationLocations",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(locationNames);

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper);

        var results = await repository
                .GetProviderNotificationLocations(providerNotificationId);

        results.Should().BeEquivalentTo(locationNames);
    }

    [Fact]
    public async Task GetAvailableLocationPostcodes_Sets_Dynamic_Parameters()
    {
        const int providerNotificationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetProviderNotificationLocations(providerNotificationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("providerNotificationId", providerNotificationId);
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
        var locationNameDtoList = new NotificationLocationNameDtoBuilder()
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
                Arg.Do<Func<NotificationSummaryDto, NotificationLocationNameDto, NotificationSummary>>(
                    x =>
                    {
                        var n = notificationSummaryDtoList[callIndex];
                        var l = locationNameDtoList[callIndex];
                        x.Invoke(n, l);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: "Id, NotificationLocationId",
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetNotificationSummaryList(TestUkPrn, includeAdditionalData))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(notificationSummaryDtoList.First());
        results[0].Locations.Should().NotBeNullOrEmpty();
        results[0].Locations.First().Validate(locationNameDtoList.First());
    }

    [Fact]
    public async Task GetNotificationSummaryList_Sets_Dynamic_Parameters()
    {
        const bool includeAdditionalData = true;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

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
                Arg.Do<Func<NotificationLocationSummaryDto, RouteDto, NotificationLocationSummary>>(
                    x =>
                    {
                        var n = notificationLocationSummaryDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(n, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var results = (await repository.GetNotificationLocationSummaryList(notificationId))
            .ToList();

        results.Should().NotBeNullOrEmpty();
        results!.Count.Should().Be(1);
        results.First().Validate(notificationLocationSummaryDtoList.First());

        results[0].Routes.Should().NotBeNullOrEmpty();
        results[0].Routes.First().Validate(routeDtoList[0]);
    }

    [Fact]
    public async Task GetNotificationLocationSummaryList_Sets_Dynamic_Parameters()
    {
        const int notificationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

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
                splitOn: "Id, RouteId",
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

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

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
    public async Task GetPendingNotificationEmails_Returns_Expected_List()
    {
        const NotificationFrequency frequency = NotificationFrequency.Daily;

        var notificationEmails = new NotificationEmailBuilder()
            .BuildList()
            .ToList();

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        dbContextWrapper
            .QueryAsync<NotificationEmail>(dbConnection,
                "GetPendingNotifications",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(notificationEmails);

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper);

        var results = await repository.GetPendingNotificationEmails(frequency);

        results.Should().BeEquivalentTo(notificationEmails);
    }

    [Fact]
    public async Task GetNotificationLocation_Returns_Expected_Results()
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
                "GetNotificationLocationDetail",
                Arg.Do<Func<NotificationDto, RouteDto, Notification>>(
                    x =>
                    {
                        var n = notificationDtoList[callIndex];
                        var r = routeDtoList[callIndex];
                        x.Invoke(n, r);

                        callIndex++;
                    }),
                Arg.Any<object>(),
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure
            );

        var repository = new NotificationRepositoryBuilder().Build(dbContextWrapper);

        var result = await repository.GetNotificationLocation(id);

        result.Validate(notificationDtoList.First());
        result.Routes.First().Id.Should().Be(routeDtoList[0].RouteId);
        result.Routes.First().Name.Should().Be(routeDtoList[0].RouteName);
    }

    [Fact]
    public async Task GetNotificationLocation_Sets_Dynamic_Parameters()
    {
        const int notificationLocationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.GetNotificationLocation(notificationLocationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("notificationLocationId", notificationLocationId);
    }

    [Fact]
    public async Task CreateNotification_Calls_Database()
    {
        const int newId = 2;

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        //var (dbContextWrapper, dbConnection) =
        //    new DbContextWrapperBuilder()
        //        .BuildSubstituteWrapperAndConnection();

        //var dynamicParametersWrapper = Substitute.For<IDynamicParametersWrapper>();
        //var parameters = new DynamicParameters();
        //parameters.Add("returnValue", newId, DbType.Int32, ParameterDirection.ReturnValue);
        //dynamicParametersWrapper.DynamicParameters.Returns(parameters);
        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();
        dynamicParametersWrapper.ReturnValue = newId;

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        var result = await repository.Create(notification, TestUkPrn);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "CreateProviderNotification"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task CreateNotification_Sets_Dynamic_Parameters()
    {
        const int newId = 2;

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();
        dynamicParametersWrapper.ReturnValue = newId;

        dbContextWrapper
            .ExecuteAsync(dbConnection,
                "CreateProviderNotification",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(newId);

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory
                //dynamicParametersWrapper
                );

        await repository.Create(notification, TestUkPrn);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(7);
        templates.ContainsNameAndValue("ukPrn", TestUkPrn);
        templates.ContainsNameAndValue("email", notification.Email);
        templates.ContainsNameAndValue("emailVerificationToken", notification.EmailVerificationToken);
        templates.ContainsNameAndValue("frequency", notification.Frequency);
        templates.ContainsNameAndValue("locationId", notification.LocationId);
        templates.ContainsNameAndValue("searchRadius", notification.SearchRadius);
        templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds")
            .Should().NotBeNull();
    }

    [Fact]
    public async Task CreateNotification_Returns_Expected_Result()
    {
        const int newId = 1;
        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();
        dynamicParametersWrapper.ReturnValue = newId;
        
        dbContextWrapper
            .ExecuteAsync(dbConnection,
                "CreateProviderNotification",
                Arg.Any<object>(),
                commandType: CommandType.StoredProcedure)
            .Returns(newId);

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        var result = await repository.Create(notification, TestUkPrn);

        result.Should().Be(newId);
    }

    [Fact]
    public async Task CreateNotificationLocation_Calls_Database()
    {
        const int providerNotificationId = 10;

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.CreateLocation(notification, providerNotificationId);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "CreateNotificationLocation"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task CreateNotificationLocation_Sets_Dynamic_Parameters()
    {
        const int providerNotificationId = 10;

        var notification = new NotificationBuilder()
            .WithNullId()
            .Build();

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.CreateLocation(notification, providerNotificationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(5);
        templates.ContainsNameAndValue("providerNotificationId", providerNotificationId);
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

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

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

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

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
    public async Task UpdateNotificationLocation_Calls_Database()
    {
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateLocation(notification);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s => s == "UpdateNotificationLocation"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task UpdateNotificationLocation_Sets_Dynamic_Parameters()
    {
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateLocation(notification);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(4);
        templates.ContainsNameAndValue("notificationLocationId", notification.Id);
        templates.ContainsNameAndValue("frequency", notification.Frequency);
        templates.ContainsNameAndValue("searchRadius", notification.SearchRadius);
        templates.GetParameter<SqlMapper.ICustomQueryParameter>("routeIds")
            .Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateNotificationSentDate_Calls_Database()
    {
        const int notificationLocationId = 1;

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateNotificationSentDate(notificationLocationId);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("UPDATE dbo.NotificationLocation") &&
                    s.Contains("SET LastNotificationDate = GETUTCDATE(),") &&
                    s.Contains("ModifiedOn = GETUTCDATE()") &&
                    s.Contains("WHERE Id = @notificationLocationId")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters));
    }

    [Fact]
    public async Task UpdateNotificationSentDate_Sets_Dynamic_Parameters()
    {
        const int notificationLocationId = 1;

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.UpdateNotificationSentDate(notificationLocationId);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("notificationLocationId", notificationLocationId);
    }

    [Fact]
    public async Task SaveEmailVerificationToken_Calls_Database()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, dbConnection, dynamicParametersWrapper) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.SaveEmailVerificationToken(notification.Id!.Value, notification.Email, verificationToken);

        await dbContextWrapper
            .Received(1)
            .ExecuteAsync(dbConnection,
                Arg.Is<string>(s =>
                    s.Contains("UPDATE dbo.ProviderNotification") &&
                    s.Contains("SET EmailVerificationToken = @emailVerificationToken,") &&
                    s.Contains("ModifiedOn = GETUTCDATE()") &&
                    s.Contains("WHERE Id = @providerNotificationId") &&
                    s.Contains("AND Email = @email")),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters));
    }

    [Fact]
    public async Task SaveEmailVerificationToken_Sets_Dynamic_Parameters()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");
        var notification = new NotificationBuilder()
            .Build();

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.SaveEmailVerificationToken(notification.Id!.Value, notification.Email, verificationToken);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(3);
        templates.ContainsNameAndValue("providerNotificationId", notification.Id);
        templates.ContainsNameAndValue("email", notification.Email);
        templates.ContainsNameAndValue("emailVerificationToken", verificationToken);
    }
    
    [Fact]
    public async Task VerifyEmailToken_Calls_Database()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");

        var (dbContextWrapper, dbConnection) = new DbContextWrapperBuilder()
            .BuildSubstituteWrapperAndConnection();

        var dynamicParametersWrapper = new DynamicParametersWrapperBuilder()
            .Build();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper);

        await repository.VerifyEmailToken(verificationToken);

        await dbContextWrapper
            .Received(1)
            .ExecuteScalarAsync<string>(dbConnection,
                Arg.Is<string>(s => s == "VerifyNotificationEmailToken"),
                Arg.Is<object>(o => o == dynamicParametersWrapper.DynamicParameters),
                commandType: CommandType.StoredProcedure);
    }

    [Fact]
    public async Task VerifyEmailToken_Sets_Dynamic_Parameters()
    {
        var verificationToken = Guid.Parse("d1cece97-ce55-4c5b-8216-2cd65490d0b2");

        var (dbContextWrapper, _, dynamicParametersWrapper) =
            new DbContextWrapperBuilder()
                .BuildSubstituteWrapperAndConnectionWithDynamicParameters();

        var repository = new NotificationRepositoryBuilder()
            .Build(dbContextWrapper,
                dynamicParametersWrapper.DapperParameterFactory);

        await repository.VerifyEmailToken(verificationToken);

        var templates = dynamicParametersWrapper.DynamicParameters.GetDynamicTemplates();
        templates.Should().NotBeNullOrEmpty();

        templates.GetDynamicTemplatesCount().Should().Be(1);
        templates.ContainsNameAndValue("emailVerificationToken", verificationToken);
    }
}