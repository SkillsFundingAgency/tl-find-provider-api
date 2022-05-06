using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Intertech.Facade.DapperParameters;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class RouteRepository : IRouteRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDapperParameters _dbParameters;

    public RouteRepository(
        IDbContextWrapper dbContextWrapper,
        IDapperParameters dbParameters)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dbParameters = dbParameters ?? throw new ArgumentNullException(nameof(dbParameters));
    }

    public async Task<IEnumerable<Route>> GetAll(bool includeAdditionalData)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dbParameters.CreateParmsWithTemplate(new
        {
            includeAdditionalData
        });

        return await _dbContextWrapper.QueryAsync<Route>(
            connection,
            "GetRoutes",
            _dbParameters.DynamicParameters,
            commandType: CommandType.StoredProcedure);
    }
}