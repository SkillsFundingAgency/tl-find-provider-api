using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
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
                    if (!notifications.TryGetValue(e.LocationId, out var notification))
                    {
                        notifications.Add(e.LocationId,
                            notification = new Notification
                            {
                                Id = e.Id,
                                LocationId = e.LocationId,
                                LocationName = e.LocationName,
                                Postcode = e.Postcode,
                                Email = e.Email,
                                SearchRadius = e.SearchRadius,
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
}
