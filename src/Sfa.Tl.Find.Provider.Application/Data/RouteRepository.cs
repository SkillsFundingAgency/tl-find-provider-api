using System.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class RouteRepository : IRouteRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;

    public RouteRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
    }

    public async Task<IEnumerable<Route>> GetAll(bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            includeAdditionalData
        });

        var routes = new Dictionary<int, Route>();

        await _dbContextWrapper.QueryAsync<RouteDto, QualificationDto, Route>(
            connection,
            "GetRoutes",
            (r, q) =>
            {
                if (!routes.TryGetValue(r.RouteId, out var routeResult))
                {
                    routes.Add(r.RouteId, 
                        routeResult = new Route
                        {
                            Id = r.RouteId, 
                            Name = r.RouteName
                        });
                }
                
                if (q is not null)
                {
                    routeResult.Qualifications.Add(
                        new Qualification
                        {
                            Id = q.QualificationId,
                            Name = q.QualificationName,
                            NumberOfQualificationsOffered = q.NumberOfQualificationsOffered
                        });
                }

                return routeResult;
            },
            _dynamicParametersWrapper.DynamicParameters,
            splitOn: "RouteId, QualificationId",
            commandType: CommandType.StoredProcedure);

        return routes
            .Values
            .OrderBy(r => r.Name)
            .ToList();
    }
}