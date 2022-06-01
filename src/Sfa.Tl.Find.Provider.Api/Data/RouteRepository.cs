using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

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

        return await _dbContextWrapper.QueryAsync<Route>(
            connection,
            "GetRoutes",
            _dynamicParametersWrapper.DynamicParameters,
            commandType: CommandType.StoredProcedure);
    }
}