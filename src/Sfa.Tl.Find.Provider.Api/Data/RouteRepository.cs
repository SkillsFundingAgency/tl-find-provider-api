using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

    public async Task<IEnumerable<Route>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<Route>(
            connection,
            "SELECT Id, Name " +
            "FROM dbo.Route " +
            "WHERE IsDeleted = 0 " +
            "ORDER BY Name");
    }
}