using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class EmployerInterestRepository : IEmployerInterestRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper; 
    private readonly ILogger<EmployerInterestRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public EmployerInterestRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<EmployerInterestRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<int> DeleteBefore(DateTime date)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();
        
        //using var transaction = _dbContextWrapper.BeginTransaction(connection);
        //var updateResult = await _dbContextWrapper
        //    .QueryAsync<(string Change, int ChangeCount)>(
        //        connection,
        //        "UpdateTowns",
        //        _dynamicParametersWrapper.DynamicParameters,
        //        transaction,
        //        commandType: CommandType.StoredProcedure);

        //_logger.LogChangeResults(updateResult, nameof(EmployerInterestRepository),
        //    nameof(towns));

        //transaction.Commit();

        return 0;
    }
}