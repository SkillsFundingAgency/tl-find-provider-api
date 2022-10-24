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
            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

            return await retryPolicy
                .ExecuteAsync(async _ =>
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
                                    //TODO: Check lengths
                                    //      Move this outside retry code
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

                    if (employerInterest.IndustryId is > 0)
                    {
                        _dynamicParametersWrapper.AddParameter("industryIds",
                            new List<int> { employerInterest.IndustryId.Value }
                                .AsTableValuedParameter("dbo.IdListTableType"));
                    }

                    if (employerInterest.SkillAreaIds != null && employerInterest.SkillAreaIds.Any())
                    {
                        _dynamicParametersWrapper.AddParameter("routeIds",
                            employerInterest.SkillAreaIds
                                .AsTableValuedParameter("dbo.IdListTableType"));
                    }

                    using var transaction = _dbContextWrapper.BeginTransaction(connection);

                    var result = await _dbContextWrapper.ExecuteAsync(
                        connection,
                        "CreateEmployerInterest",
                        _dynamicParametersWrapper.DynamicParameters,
                        transaction,
                        commandType: CommandType.StoredProcedure);

                    transaction.Commit();

                    return (result, uniqueId);
                },
            context);
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

            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

            return await retryPolicy
                .ExecuteAsync(async _ =>
                    {
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

                            await _dbContextWrapper.ExecuteAsync(
                                connection,
                                "DeleteEmployerInterest",
                                _dynamicParametersWrapper.DynamicParameters,
                                transaction,
                                commandType: CommandType.StoredProcedure);

                            result = 1;
                        }

                        transaction.Commit();
                        return result;
                    },
                    context);
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
                 new { date },
                transaction
            ))?.ToList();

            if (idsToDelete != null && idsToDelete.Any())
            {
                _dynamicParametersWrapper.CreateParameters(new
                {
                    employerInterestIds = idsToDelete
                        .AsTableValuedParameter("dbo.IdListTableType")
                });

                await _dbContextWrapper.ExecuteAsync(
                    connection,
                    "DeleteEmployerInterest",
                    _dynamicParametersWrapper.DynamicParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                result = idsToDelete.Count;
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

    public async Task<EmployerInterestDetail> GetDetail(int id)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            id
        });

        EmployerInterestDetail detailItem = null;

        await _dbContextWrapper
                .QueryAsync<EmployerInterestDetailDto, RouteDto, EmployerInterestDetail>(
                    connection,
                    "GetEmployerInterestDetail",
                    (e, r) =>
                    {
                        detailItem ??= new EmployerInterestDetail
                        {
                            Id = e.Id,
                            UniqueId = e.UniqueId,
                            OrganisationName = e.OrganisationName,
                            ContactName = e.ContactName,
                            Postcode = e.Postcode,
                            Latitude = e.Latitude,
                            Longitude = e.Longitude,
                            Industry = e.Industry,
                            AdditionalInformation = e.AdditionalInformation,
                            Email = e.Email,
                            Telephone = e.Telephone,
                            Website = e.Website,
                            ContactPreferenceType = e.ContactPreferenceType,
                            CreatedOn = e.CreatedOn,
                            ModifiedOn = e.ModifiedOn,
                            SkillAreas = new List<string>()
                        };

                        if (r is not null)
                        {
                            detailItem.SkillAreas.Add(r.RouteName);
                        }

                        return detailItem;
                    },
                    _dynamicParametersWrapper.DynamicParameters,
                    splitOn: "Id, RouteId",
                    commandType: CommandType.StoredProcedure);

        return detailItem;
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

        var summaryList = new Dictionary<int, EmployerInterestSummary>();

        await _dbContextWrapper
            .QueryAsync<EmployerInterestSummaryDto, RouteDto, EmployerInterestSummary>(
                connection,
                "GetEmployerInterestSummary",
                (e, r) =>
                {
                    if (!summaryList.TryGetValue(e.Id, out var summaryItem))
                    {
                        summaryList.Add(e.Id,
                            summaryItem = new EmployerInterestSummary
                            {
                                Id = e.Id,
                                OrganisationName = e.OrganisationName,
                                Distance = e.Distance,
                                Industry = e.Industry,
                                CreatedOn = e.CreatedOn,
                                ModifiedOn = e.ModifiedOn,
                                SkillAreas = new List<string>()
                            });
                    }

                    if (r is not null)
                    {
                        summaryItem.SkillAreas.Add(r.RouteName);
                    }

                    return summaryItem;
                },
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return summaryList
            .Values
            .OrderByDescending(e => e.CreatedOn)
            .ThenBy(e => e.OrganisationName)
            .ToList();
    }
}