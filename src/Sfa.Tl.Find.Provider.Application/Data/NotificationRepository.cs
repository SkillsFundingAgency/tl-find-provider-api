﻿using System.Data;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class NotificationRepository : INotificationRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        ILogger<NotificationRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ??
                                    throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Create(Notification notification, long ukPrn)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            var routeIds = notification.Routes
                .Select(r => r.Id)
                .AsTableValuedParameter("dbo.IdListTableType");

            _dynamicParametersWrapper.CreateParameters(new
            {
                ukPrn,
                email = notification.Email,
                emailVerificationToken = notification.EmailVerificationToken,
                frequency = notification.Frequency,
                searchRadius = notification.SearchRadius,
                locationId = notification.LocationId,
                routeIds
            });
            _dynamicParametersWrapper
                .AddReturnValueParameter("returnValue", DbType.Int32);

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateProviderNotification",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);

            var providerNotificationId = _dynamicParametersWrapper
                .DynamicParameters
                .Get<int>("returnValue");

            return providerNotificationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when creating a notification");
            throw;
        }
    }

    public async Task CreateLocation(Notification notification, int providerNotificationId)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            var routeIds = notification.Routes
                .Select(r => r.Id)
                .AsTableValuedParameter("dbo.IdListTableType");

            _dynamicParametersWrapper.CreateParameters(new
            {
                providerNotificationId,
                frequency = notification.Frequency,
                searchRadius = notification.SearchRadius,
                locationId = notification.LocationId,
                routeIds
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateNotificationLocation",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when updating a notification");
            throw;
        }
    }

    public async Task Delete(int providerNotificationId)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            providerNotificationId
        });

        await _dbContextWrapper.ExecuteAsync(
            connection,
            "DeleteProviderNotification",
            _dynamicParametersWrapper.DynamicParameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteLocation(int notificationLocationId)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            notificationLocationId
        });

        await _dbContextWrapper.ExecuteAsync(
            connection,
            "DeleteNotificationLocation",
            _dynamicParametersWrapper.DynamicParameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<NotificationLocationName>> GetProviderNotificationLocations(int providerNotificationId)
    {
        if (providerNotificationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(providerNotificationId));
        }

        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            providerNotificationId
        });

        return await _dbContextWrapper
            .QueryAsync<NotificationLocationName>(
                connection,
                "GetProviderNotificationLocations",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
    }

    public async Task<DateTime?> GetLastNotificationSentDate(IEnumerable<int> idList)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                idList
            });

            return await _dbContextWrapper.ExecuteScalarAsync<DateTime?>(
                connection,
                "SELECT MAX(LastNotificationDate) " +
                "FROM NotificationLocation " +
                "WHERE ID in @idList",
                _dynamicParametersWrapper.DynamicParameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the last notification date");
            throw;
        }
    }

    public async Task<Notification> GetNotification(
        int notificationId)
    {
        if (notificationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(notificationId));
        }

        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            notificationId
        });

        Notification notification = null;

        await _dbContextWrapper
            .QueryAsync<NotificationDto, RouteDto, Notification>(
                connection,
                "GetNotificationDetail",
                (n, r) =>
                {
                    notification ??= new Notification
                    {
                        Id = n.Id,
                        Email = n.Email,
                        IsEmailVerified = n.IsEmailVerified,
                        Frequency = n.Frequency,
                        SearchRadius = n.SearchRadius,
                        LocationId = n.LocationId,
                        LocationName = n.LocationName,
                        Postcode = n.Postcode,
                        Routes = new List<Route>()
                    };

                    if (r is not null)
                    {
                        notification.Routes.Add(
                            new Route
                            {
                                Id = r.RouteId,
                                Name = r.RouteName
                            });
                    }

                    return notification;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return notification;
    }

    public async Task<IEnumerable<NotificationEmail>> GetPendingNotificationEmails(NotificationFrequency frequency)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            _dynamicParametersWrapper.CreateParameters(new
            {
                frequency
            });

            var results = await _dbContextWrapper
                .QueryAsync<NotificationEmail>(
                    connection,
                    "GetPendingNotificationsWithUpdate",
                    _dynamicParametersWrapper.DynamicParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

            transaction.Commit();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when getting pending notifications for frequency {frequency}", frequency);
            throw;
        }
    }

    public async Task<Notification> GetNotificationLocation(
        int notificationLocationId)
    {
        if (notificationLocationId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(notificationLocationId));
        }

        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            notificationLocationId
        });

        Notification notification = null;

        await _dbContextWrapper
            .QueryAsync<NotificationDto, RouteDto, Notification>(
                connection,
                "GetNotificationLocationDetail",
                (n, r) =>
                {
                    notification ??= new Notification
                    {
                        Id = n.Id,
                        Email = n.Email,
                        IsEmailVerified = n.IsEmailVerified,
                        Frequency = n.Frequency,
                        SearchRadius = n.SearchRadius,
                        LocationId = n.LocationId,
                        LocationName = n.LocationName,
                        Postcode = n.Postcode,
                        Routes = new List<Route>()
                    };

                    if (r is not null)
                    {
                        notification.Routes.Add(
                            new Route
                            {
                                Id = r.RouteId,
                                Name = r.RouteName
                            });
                    }

                    return notification;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return notification;
    }

    public async Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn
        });

        var notifications = new Dictionary<int, NotificationSummary>();

        await _dbContextWrapper
            .QueryAsync<NotificationSummaryDto, NotificationLocationNameDto, NotificationSummary>(
                connection,
                "GetNotificationSummary",
                (n, l) =>
                {
                    if (!notifications.TryGetValue(n.Id!.Value, out var notification))
                    {
                        notifications.Add(n.Id.Value,
                            notification = new NotificationSummary
                            {
                                Id = n.Id,
                                Email = n.Email,
                                IsEmailVerified = n.IsEmailVerified,
                                Locations = new List<NotificationLocationName>()
                            });
                    }

                    if (l is not null && l.NotificationLocationId is not null)
                    {
                        notification.Locations.Add(
                            new NotificationLocationName
                            {
                                Id = l.NotificationLocationId,
                                LocationId = l.LocationId,
                                Name = l.LocationName,
                                Postcode = l.Postcode
                            });
                    }

                    return notification;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, NotificationLocationId",
                commandType: CommandType.StoredProcedure);

        return notifications.Values;
    }

    public async Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(
        int notificationId)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            notificationId
        });

        var notifications = new Dictionary<int, NotificationLocationSummary>();

        await _dbContextWrapper
            .QueryAsync<NotificationLocationSummaryDto, RouteDto, NotificationLocationSummary>(
                connection,
                "GetNotificationLocationSummary",
                (n, r) =>
                {
                    if (!notifications.TryGetValue(n.Id!.Value, out var notification))
                    {
                        notifications.Add(n.Id.Value,
                            notification = new NotificationLocationSummary
                            {
                                Id = n.Id,
                                SearchRadius = n.SearchRadius,
                                Frequency = n.Frequency,
                                Location = n.LocationId is not null ?
                                    new NotificationLocationName
                                    {
                                        Id = n.Id,
                                        Name = n.LocationName,
                                        Postcode = n.Postcode,
                                    }
                                : null,
                                Routes = new List<Route>()
                            });
                    }

                    if (r is not null && notification.Routes.All(x => x.Id != r.RouteId))
                    {
                        notification.Routes.Add(
                            new Route
                            {
                                Id = r.RouteId,
                                Name = r.RouteName
                            });
                    }

                    return notification;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return notifications.Values;
    }

    public async Task Update(Notification notification)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            var routeIds = notification.Routes
                .Select(r => r.Id)
                .AsTableValuedParameter("dbo.IdListTableType");

            _dynamicParametersWrapper.CreateParameters(new
            {
                id = notification.Id.Value,
                email = notification.Email,
                frequency = notification.Frequency,
                searchRadius = notification.SearchRadius,
                locationId = notification.LocationId,
                routeIds
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "UpdateNotification",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when updating a notification");
            throw;
        }
    }

    public async Task UpdateLocation(Notification notification)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            var routeIds = notification.Routes
                .Select(r => r.Id)
                .AsTableValuedParameter("dbo.IdListTableType");

            _dynamicParametersWrapper.CreateParameters(new
            {
                notificationLocationId = notification.Id.Value,
                frequency = notification.Frequency,
                searchRadius = notification.SearchRadius,
                routeIds
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "UpdateNotificationLocation",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when updating a notification");
            throw;
        }
    }

    public async Task UpdateNotificationSentDate(
        IEnumerable<int> notificationLocationIds,
        DateTime notificationDate)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                ids = notificationLocationIds
                    .AsTableValuedParameter("dbo.IdListTableType"),
                notificationDate
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "UPDATE dbo.NotificationLocation " +
                "SET LastNotificationSentDate = @notificationDate, " +
                "    ModifiedOn = GETUTCDATE() " +
                "WHERE Id IN (SELECT Id FROM @ids) ",
                _dynamicParametersWrapper.DynamicParameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving a notification last sent date");
            throw;
        }
    }

    public async Task SaveEmailVerificationToken(
        int providerNotificationId,
        string emailAddress,
        Guid? emailVerificationToken)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                providerNotificationId,
                email = emailAddress,
                emailVerificationToken
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "UPDATE dbo.ProviderNotification " +
                "SET EmailVerificationToken = @emailVerificationToken, " +
                "    ModifiedOn = GETUTCDATE() " +
                "WHERE Id = @providerNotificationId " +
                "  AND Email = @email",
                _dynamicParametersWrapper.DynamicParameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving a notification email verification token");
            throw;
        }
    }

    public async Task<(bool Success, string Email)> VerifyEmailToken(Guid emailVerificationToken)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                emailVerificationToken
            });

            var email = await _dbContextWrapper.ExecuteScalarAsync<string>(
                connection,
                "VerifyNotificationEmailToken",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);

            return (email is not null, email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving a notification email verification token");
            throw;
        }
    }
}
