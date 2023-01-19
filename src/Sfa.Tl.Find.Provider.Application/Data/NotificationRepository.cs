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
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
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

    public async Task<IEnumerable<Notification>> GetNotifications(long ukPrn, bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            ukPrn,
            includeAdditionalData
        });

        var notifications = new Dictionary<int, Notification>();

        await _dbContextWrapper
            .QueryAsync<NotificationDto, RouteDto, Notification>(
                connection,
                "GetNotifications",
                (e, r) =>
                {
                    if (!notifications.TryGetValue(e.Id!.Value, out var notification))
                    {
                        notifications.Add(e.Id.Value,
                            notification = new Notification
                            {
                                Id = e.Id,
                                Email = e.Email,
                                Frequency = e.Frequency,
                                SearchRadius = e.SearchRadius,
                                LocationId = e.LocationId,
                                LocationName = e.LocationName,
                                Postcode = e.Postcode,
                                Routes = new List<Route>()
                            });
                    }

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

        return notifications.Values;
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
                (e, r) =>
                {
                    notification ??= new Notification
                    {
                        Id = e.Id,
                        Email = e.Email,
                        Frequency = e.Frequency,
                        SearchRadius = e.SearchRadius,
                        LocationId = e.LocationId,
                        LocationName = e.LocationName,
                        Postcode = e.Postcode,
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

    public async Task Save(Notification notification)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();

            _dynamicParametersWrapper.CreateParameters(new
            {
                locationId = notification.LocationId,
                searchRadius = notification.SearchRadius,
                routeIds = notification.Routes
                    .Select(r => r.Id)
                    .AsTableValuedParameter("dbo.IdListTableType")
            });

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateOrUpdateNotification",
                _dynamicParametersWrapper.DynamicParameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving employer interest");
            throw;
        }
    }
}
