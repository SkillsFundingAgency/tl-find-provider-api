using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class TownRepository : ITownRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper; 
    private readonly ILogger<TownRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public TownRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<TownRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasAny()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var result = await _dbContextWrapper.ExecuteScalarAsync<int>(
            connection,
            "SELECT COUNT(1) " +
            "WHERE EXISTS (SELECT 1 FROM dbo.Town)");

        return result != 0;
    }

    public async Task<IEnumerable<Town>> Search(string searchTerms,
        int maxResults)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            maxResults,
            query = $"{searchTerms.ToSearchableString()}%"
        });

        var results = await _dbContextWrapper.QueryAsync<Town>(
            connection,
            "SELECT TOP (@maxResults) " +
            "            [Id], " +
            "            [Name], " +
            "            [County], " +
            "            [LocalAuthority], " +
            "            [Latitude], " +
            "            [Longitude] " +
            "FROM dbo.[TownSearchView] " +
            "WITH(NOEXPAND) " +
            "WHERE [Search] LIKE @query",
            _dynamicParametersWrapper.DynamicParameters);

        return results;
    }

    public async Task Save(IEnumerable<Town> towns)
    {
        try
        {
            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

            await retryPolicy
                .ExecuteAsync(async _ =>
                        await PerformSave(towns),
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving towns");
            throw;
        }
    }

    private async Task PerformSave(IEnumerable<Town> towns)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        _dynamicParametersWrapper.CreateParameters(new
        {
            data = towns.AsTableValuedParameter(
                "dbo.TownDataTableType")
        });

        using var transaction = _dbContextWrapper.BeginTransaction(connection);
        var updateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateTowns",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(updateResult, nameof(TownRepository),
            nameof(towns));

        transaction.Commit();
    }
}