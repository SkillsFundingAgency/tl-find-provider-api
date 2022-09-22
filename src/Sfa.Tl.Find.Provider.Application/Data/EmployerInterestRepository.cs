﻿using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

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

    public async Task<Guid> Create(EmployerInterest employerInterest)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        _dynamicParametersWrapper.CreateParameters(new
        {
            employerInterest.UniqueId,
            employerInterest.OrganisationName,
            employerInterest.ContactName,
            employerInterest.Postcode,
            employerInterest.HasMultipleLocations,
            employerInterest.LocationCount,
            employerInterest.IndustryId,
            employerInterest.SpecificRequirements,
            employerInterest.Email,
            employerInterest.Telephone,
            employerInterest.ContactPreferenceType
        });
        var sql = "INSERT INTO dbo.EmployerInterest " +
                  "(UniqueId, " +
                  " OrganisationName, " +
                  " ContactName, " +
                  " Postcode, " +
                  " HasMultipleLocations, " +
                  " LocationCount, " +
                  " IndustryId, " +
                  " SpecificRequirements, " +
                  " Email, " +
                  " Telephone, " +
                  " ContactPreferenceType) " +
                  "VALUES (" +
                  " @uniqueId, " +
                  " @organisationName, " +
                  " @contactName, " +
                  " @postcode, " +
                  " @hasMultipleLocations, " +
                  " @locationCount, " +
                  " @industryId, " +
                  " @specificRequirements, " +
                  " @email, " +
                  " @telephone, " +
                  " @contactPreferenceType" +
                  ")";

        using var transaction = _dbContextWrapper.BeginTransaction(connection);

        var result = await _dbContextWrapper.ExecuteAsync(
            connection,
            sql,
            _dynamicParametersWrapper.DynamicParameters,
            transaction);

        transaction.Commit();

        return employerInterest.UniqueId;
    }

    public async Task<int> Delete(Guid uniqueId)
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
            "DELETE dbo.EmployerInterest " +
            "WHERE UniqueId = @uniqueId",
            _dynamicParametersWrapper.DynamicParameters,
            transaction);

        transaction.Commit();

        return result;
    }

    public async Task<int> DeleteBefore(DateTime date)
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
            "DELETE dbo.EmployerInterest " +
            "WHERE CreatedOn < @date",
            _dynamicParametersWrapper.DynamicParameters,
            transaction);
        
        transaction.Commit();

        return result;
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