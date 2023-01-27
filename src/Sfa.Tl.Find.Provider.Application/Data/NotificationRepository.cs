using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class NotificationRepository : INotificationRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<NotificationRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public NotificationRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<NotificationRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ??
                                    throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Delete(int notificationId)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            notificationId
        });

        await _dbContextWrapper.ExecuteAsync(
            connection,
            "DeleteNotification",
            _dynamicParametersWrapper.DynamicParameters,
            commandType: CommandType.StoredProcedure);
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

    public async Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn,
        bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn,
            includeAdditionalData
        });

        var notifications = new Dictionary<int, NotificationSummary>();

        await _dbContextWrapper
            .QueryAsync<NotificationSummaryDto, LocationPostcodeDto, NotificationSummary>(
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
                                Locations = new List<LocationPostcode>()
                            });
                    }

                    if (l is not null)
                    {
                        notification.Locations.Add(
                            new LocationPostcode
                            {
                                Id = l.LocationId,
                                Name = l.LocationName,
                                Postcode = l.Postcode
                            });
                    }

                    return notification;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, LocationId",
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
                                    new LocationPostcode
                                    {
                                        Id = n.LocationId,
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


    public async Task Create(Notification notification, long ukPrn)
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

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateProviderNotification",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when creating a notification");
            throw;
        }
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

            var result = await _dbContextWrapper.ExecuteAsync(
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

    public async Task RemoveEmailVerificationToken(
        Guid emailVerificationToken)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                emailVerificationToken
            });

            await _dbContextWrapper.ExecuteAsync(
                connection,
                "UPDATE dbo.ProviderNotification " +
                "SET EmailVerificationToken = NULL, " +
                "    ModifiedOn = GETUTCDATE() " +
                "WHERE EmailVerificationToken = @emailVerificationToken",
                _dynamicParametersWrapper.DynamicParameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving a notification email verification token");
            throw;
        }
    }
}
