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

    public RouteRepository(
        IDbContextWrapper dbContextWrapper)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
    }

    public async Task<IEnumerable<Route>> GetAll(bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<Route>(
            connection,
            "GetRoutes",
            new
            {
                includeAdditionalData
            },
            commandType: CommandType.StoredProcedure);
    }
}