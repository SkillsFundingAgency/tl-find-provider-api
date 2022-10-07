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
                            HasMultipleLocations = employerInterest.HasMultipleLocations,
                            LocationCount = employerInterest.LocationCount,
                            IndustryId = employerInterest.IndustryId,
                            SpecificRequirements = employerInterest.SpecificRequirements,
                            Email = employerInterest.Email,
                            Telephone = employerInterest.Telephone,
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
            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            _dynamicParametersWrapper.CreateParameters(new
            {
                uniqueId
            });

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "DeleteEmployerInterest",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);
            
            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest");
            throw;
        }
    }

    public async Task<int> DeleteBefore(DateTime date)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            _dynamicParametersWrapper.CreateParameters(new
            {
                date
            });

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            var result = await _dbContextWrapper.ExecuteAsync(
                connection,
                "DeleteEmployerInterestBeforeDate",
                _dynamicParametersWrapper.DynamicParameters,
                transaction,
                commandType: CommandType.StoredProcedure);

            transaction.Commit();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest");
            throw;
        }
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
            "SpecificRequirements, " +
            "Email, " +
            "Telephone, " +
            "ContactPreferenceType, " +
            "CreatedOn, " +
            "ModifiedOn " +
            "FROM dbo.EmployerInterest " +
            "ORDER BY OrganisationName");
    }
}