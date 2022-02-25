using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class ProviderReferenceRepository : IProviderReferenceRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly ILogger<ProviderReferenceRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public ProviderReferenceRepository(
        IDbContextWrapper dbContextWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<ProviderReferenceRepository> logger)
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
            "WHERE EXISTS (SELECT 1 FROM dbo.ProviderReference)");

        return result != 0;
    }

    public async Task<IEnumerable<ProviderReference>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<ProviderReference>(
            connection,
            "SELECT Id, UkPrn, Urn, Name " +
            "FROM dbo.ProviderReference " +
            "ORDER BY Name");
    }

    public async Task<ProviderReference> GetByUkPrn(long ukprn)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var result = await _dbContextWrapper.QueryAsync<ProviderReference>(
            connection,
            "SELECT Id, UkPrn, Urn, Name " +
            "FROM dbo.ProviderReference " +
            "WHERE UkPrn = @ukprn",
            //new { ukprn });
            ukprn);

        return result.FirstOrDefault();
    }

    public async Task Save(IEnumerable<ProviderReference> providerReferences)
    {
        try
        {
            var (retryPolicy, context) = _policyRegistry.GetRetryPolicy(_logger);

            await retryPolicy
                .ExecuteAsync(async _ =>
                    await PerformSave(providerReferences),
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving provider references");
            throw;
        }
    }

    private async Task PerformSave(IEnumerable<ProviderReference> providerReferences)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        using var transaction = _dbContextWrapper.BeginTransaction(connection);
        var updateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateProviderReferences",
                new
                {
                    data = providerReferences.AsTableValuedParameter(
                        "dbo.ProviderReferenceDataTableType")
                },
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(updateResult, nameof(ProviderReferenceRepository),
            nameof(providerReferences));

        transaction.Commit();
    }
}