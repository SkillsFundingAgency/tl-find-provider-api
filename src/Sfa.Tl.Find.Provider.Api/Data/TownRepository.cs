using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class TownRepository : ITownRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly ILogger<TownRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public TownRepository(
        IDbContextWrapper dbContextWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<TownRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
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

    public async Task Save(IEnumerable<Town> towns)
    {
        try
        {
            var (retryPolicy, context) = _policyRegistry.GetRetryPolicy(_logger);

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

        using var transaction = _dbContextWrapper.BeginTransaction(connection);
        var updateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateTowns",
                new
                {
                    data = towns.AsTableValuedParameter(
                        "dbo.TownDataTableType")
                },
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(updateResult, nameof(QualificationRepository),
            nameof(towns));

        transaction.Commit();
    }
}