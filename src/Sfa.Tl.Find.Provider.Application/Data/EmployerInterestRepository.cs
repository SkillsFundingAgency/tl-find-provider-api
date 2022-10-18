using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class EmployerInterestRepository : IEmployerInterestRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly IGuidService _guidService;
    private readonly ILogger<EmployerInterestRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public EmployerInterestRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        IGuidService guidService,
        ILogger<EmployerInterestRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _guidService = guidService ?? throw new ArgumentNullException(nameof(guidService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(int, Guid)> Create(EmployerInterest employerInterest)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            var uniqueId = _guidService.NewGuid();

            _dynamicParametersWrapper.CreateParameters(new
            {
                data = new List<EmployerInterestDto>
                    {
                        new()
                        {
                            UniqueId = uniqueId,
                            OrganisationName = employerInterest.OrganisationName,
                            ContactName = employerInterest.ContactName,
                            Postcode = employerInterest.Postcode,
                            Latitude = employerInterest.Latitude,
                            Longitude = employerInterest.Longitude,
                            IndustryId = employerInterest.IndustryId ?? 0,
                            AdditionalInformation = employerInterest.AdditionalInformation,
                            Email = employerInterest.Email,
                            Telephone = employerInterest.Telephone,
                            Website = employerInterest.Website,
                            ContactPreferenceType = employerInterest.ContactPreferenceType
                        }
                    }
                    .AsTableValuedParameter("dbo.EmployerInterestDataTableType")
            });

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "CreateEmployerInterest",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

            transaction.Commit();

            return (result, uniqueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving employer interest");
            throw;
        }
    }

    public async Task<int> Delete(Guid uniqueId)
    {
        try
        {
            var result = 0;

            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            var id = await _dbContextWrapper.ExecuteScalarAsync<int?>(
                connection,
                "SELECT Id " +
                "FROM [dbo].[EmployerInterest] " +
                "WHERE [UniqueId] = @uniqueId",
                new { uniqueId },
                transaction
            );

            if (id.HasValue)
            {
                _dynamicParametersWrapper.CreateParameters(new
                {
                    employerInterestIds = 
                        new List<int> { id.Value }
                            .AsTableValuedParameter("dbo.IdListTableType")
                });

                result = await _dbContextWrapper.ExecuteAsync(
                    connection,
                    "DeleteEmployerInterest",
                    _dynamicParametersWrapper.DynamicParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest with uniqueId '{uniqueId}'", uniqueId);
            throw;
        }
    }

    public async Task<int> DeleteBefore(DateTime date)
    {
        try
        {
            var result = 0;

            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();
            
            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            _dynamicParametersWrapper.CreateParameters(new
            {
                date
            });

            //TODO: Add logic to allow users to ask for extension on date - 
            //      Maybe add a flag to the table for use here, together with the ModifiedOn" +
            //      "WHERE([CreatedOn] < @date ",
            //    "  --AND [ModifiedOn] IS NULL)" +
            //    "  --OR [ModifiedOn] < @date";
            var idsToDelete = (await _dbContextWrapper.QueryAsync<int>(
                connection,
                "SELECT Id " +
                "FROM [dbo].[EmployerInterest] " +
                "WHERE [CreatedOn] < @date",
                 new { date},
                transaction
            ))?.ToList();
            
            if (idsToDelete != null && idsToDelete.Any())
            {
                _dynamicParametersWrapper.CreateParameters(new
                {
                    employerInterestIds = idsToDelete
                        .AsTableValuedParameter("dbo.IdListTableType")
                });

                result = await _dbContextWrapper.ExecuteAsync(
                    connection,
                    "DeleteEmployerInterest",
                    _dynamicParametersWrapper.DynamicParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest before date {date}", date);
            throw;
        }
    }

    public async Task<EmployerInterest> Get(int id)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            id
        });

        return (await _dbContextWrapper
                .QueryAsync<EmployerInterest>(
                    connection,
                    "SELECT Id," +
                    "UniqueId, " +
                    "OrganisationName, " +
                    "ContactName, " +
                    "Postcode, " +
                    "IndustryId, " +
                    "AdditionalInformation, " +
                    "Email, " +
                    "Telephone, " +
                    "ContactPreferenceType " +
                    "FROM dbo.EmployerInterest " +
                    "WHERE Id = @id",
                    _dynamicParametersWrapper.DynamicParameters))
            .SingleOrDefault();
    }

    public async Task<IEnumerable<EmployerInterest>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<EmployerInterest>(
            connection,
            "SELECT Id, " +
            "UniqueId, " +
            "OrganisationName, " +
            "ContactName, " +
            "Postcode, " +
            "HasMultipleLocations, " +
            "LocationCount, " +
            "IndustryId,  " +
            "AdditionalInformation, " +
            "Email, " +
            "Telephone, " +
            "ContactPreferenceType, " +
            "CreatedOn, " +
            "ModifiedOn " +
            "FROM dbo.EmployerInterest " +
            "ORDER BY OrganisationName");
    }

    public async Task<IEnumerable<EmployerInterestSummary>> GetSummaryList()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper
            .QueryAsync<EmployerInterestSummary>(
                connection,
                "GetAllEmployerInterest",
                commandType: CommandType.StoredProcedure);
    }
}