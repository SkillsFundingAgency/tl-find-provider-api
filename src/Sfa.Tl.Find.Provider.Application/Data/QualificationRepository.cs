using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class QualificationRepository : IQualificationRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<QualificationRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public QualificationRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<QualificationRepository> logger)
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
            "WHERE EXISTS (SELECT 1 FROM dbo.Qualification)");

        return result != 0;
    }

    public async Task<IEnumerable<Qualification>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<Qualification>(
            connection,
            "SELECT Id, Name " +
            "FROM dbo.Qualification " +
            "WHERE IsDeleted = 0 " +
            "ORDER BY Name");
    }

    public async Task Save(IEnumerable<Qualification> qualifications)
    {
        try
        {
            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

            await retryPolicy
                .ExecuteAsync(async _ => 
                        await PerformSave(qualifications),
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving qualifications");
            throw;
        }
    }

    private async Task PerformSave(IEnumerable<Qualification> qualifications)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        _dynamicParametersWrapper.CreateParameters(new
        {
            data = qualifications.AsTableValuedParameter(
                "dbo.QualificationDataTableType")
        });

        using var transaction = _dbContextWrapper.BeginTransaction(connection);
        var updateResult = await _dbContextWrapper
            .QueryAsync<(string Change, int ChangeCount)>(
                connection,
                "UpdateQualifications",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

        _logger.LogChangeResults(updateResult, nameof(QualificationRepository),
            nameof(qualifications));

        transaction.Commit();
    }
}