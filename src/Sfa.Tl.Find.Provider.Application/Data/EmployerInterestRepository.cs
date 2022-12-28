using System.Data;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class EmployerInterestRepository : IEmployerInterestRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly IGuidProvider _guidProvider;
    private readonly ILogger<EmployerInterestRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public EmployerInterestRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        IGuidProvider guidProvider,
        ILogger<EmployerInterestRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(int, Guid)> Create(
        EmployerInterest employerInterest,
        GeoLocation geoLocation,
        DateTime expiryDate)
    {
        try
        {
            var uniqueId = _guidProvider.NewGuid();
            _dynamicParametersWrapper.CreateParameters(new
            {
                data = new List<EmployerInterestDto>
                    {
                        new()
                        {
                            UniqueId = uniqueId,
                            OrganisationName = employerInterest.OrganisationName,
                            ContactName = employerInterest.ContactName,
                            Postcode = geoLocation.Location,
                            Latitude = geoLocation.Latitude,
                            Longitude = geoLocation.Longitude,
                            OtherIndustry = employerInterest.OtherIndustry,
                            AdditionalInformation = employerInterest.AdditionalInformation,
                            Email = employerInterest.Email,
                            Telephone = employerInterest.Telephone,
                            Website = employerInterest.Website,
                            ContactPreferenceType = (int?)employerInterest.ContactPreferenceType,
                            ExpiryDate = expiryDate
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

            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);
            return await retryPolicy
                .ExecuteAsync(async _ =>
                    {
                        using var connection = _dbContextWrapper.CreateConnection();
                        connection.Open();

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

    public async Task<int> Delete(int id)
    {
        try
        {
            var (retryPolicy, context) = _policyRegistry.GetDapperRetryPolicy(_logger);

            return await retryPolicy
                .ExecuteAsync(async _ =>
                    {
                        using var connection = _dbContextWrapper.CreateConnection();
                        connection.Open();

                        using var transaction = _dbContextWrapper.BeginTransaction(connection);

                        var employerInterestsDeleted = await PerformDelete(new List<int> { id }, connection, transaction);

                        transaction.Commit();
                        return employerInterestsDeleted;
                    },
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest with id '{id}'", id);
            throw;
        }
    }

    public async Task<int> Delete(Guid uniqueId)
    {
        try
        {
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

                        var employerInterestsDeleted = id.HasValue
                            ? await PerformDelete(new List<int> { id.Value }, connection, transaction)
                            : 0;

                        transaction.Commit();
                        return employerInterestsDeleted;
                    },
                    context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest with uniqueId '{uniqueId}'", uniqueId);
            throw;
        }
    }

    public async Task<IEnumerable<ExpiredEmployerInterestDto>> DeleteExpired(DateTime date)
    {
        try
        {
            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            _dynamicParametersWrapper.CreateParameters(new
            {
                date
            });

            var itemsToDelete = (await 
                _dbContextWrapper.QueryAsync<ExpiredEmployerInterestDto>(
                    connection,
                    "SELECT Id, UniqueId, Email " +
                    "FROM [dbo].[EmployerInterest] " +
                    "WHERE [ExpiryDate] < @date",
                     new { date },
                    transaction
                    ))?.ToList();

           if(itemsToDelete != null && itemsToDelete.Any())
           {
               await PerformDelete(itemsToDelete.Select(x => x.Id), 
                   connection, transaction);
           }

            transaction.Commit();

            return itemsToDelete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when deleting employer interest before date {date}", date);
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
            "AdditionalInformation, " +
            "Email, " +
            "Telephone, " +
            "ContactPreferenceType, " +
            "ExpiryDate, " +
            "CreatedOn, " +
            "ModifiedOn " +
            "FROM dbo.EmployerInterest " +
            "ORDER BY OrganisationName");
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
                            ExpiryDate = e.ExpiryDate,
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

    public async Task<IEnumerable<EmployerInterest>> GetExpiringInterest(int daysToExpiry)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var employerInterestList = new List<EmployerInterest>();

        _dynamicParametersWrapper.CreateParameters(new
        {
            daysToExpiry
        });

        var employerInterests = new Dictionary<Guid, EmployerInterest>();

        await _dbContextWrapper
            .QueryAsync<EmployerInterest, int, EmployerInterest>(
                connection,
                "GetExpiringEmployerInterest",
                (e, r) =>
                {
                    if (!employerInterests.TryGetValue(e.UniqueId, out var employerInterestItem))
                    {
                        employerInterests.Add(e.UniqueId,
                            employerInterestItem = new EmployerInterest
                            {
                                Id = e.Id,
                                UniqueId = e.UniqueId,
                                OrganisationName = e.OrganisationName,
                                ContactName = e.ContactName,
                                Postcode = e.Postcode,
                                IndustryId = e.IndustryId,
                                OtherIndustry = e.OtherIndustry,
                                AdditionalInformation = e.AdditionalInformation,
                                Email = e.Email,
                                Telephone = e.Telephone,
                                Website = e.Website,
                                ContactPreferenceType = e.ContactPreferenceType,
                                SkillAreaIds = new List<int>()
                            });
                    }

                    employerInterestItem.SkillAreaIds!.Add(r);

                    return employerInterestItem;
                },
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        return employerInterests.Values;
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
                                Postcode = e.Postcode,
                                Distance = e.Distance,
                                Industry = e.Industry,
                                ExpiryDate = e.ExpiryDate,
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
            .OrderBy(e => e.OrganisationName)
            .ThenByDescending(e => e.CreatedOn)
            .ToList();
    }

    public async Task<(IEnumerable<EmployerInterestSummary> SearchResults, int TotalResultsCount)> Search(
        double latitude,
        double longitude,
        int searchRadius)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            fromLatitude = latitude,
            fromLongitude = longitude,
            searchRadius
        })
            .AddOutputParameter("@totalEmployerInterestsCount", DbType.Int32);

        var summaryList = new Dictionary<int, EmployerInterestSummary>();

        await _dbContextWrapper
            .QueryAsync<EmployerInterestSummaryDto, RouteDto, EmployerInterestSummary>(
                connection,
                "SearchEmployerInterest",
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
                                ExpiryDate = e.ExpiryDate,
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
                _dynamicParametersWrapper.DynamicParameters,
                splitOn: "Id, RouteId",
                commandType: CommandType.StoredProcedure);

        var totalEmployerInterestsCount = _dynamicParametersWrapper
            .DynamicParameters
            .Get<int>("@totalEmployerInterestsCount");

        var searchResults = summaryList
            .Values
            .OrderBy(e => e.Distance)
            .ThenByDescending(e => e.CreatedOn)
            .ThenBy(e => e.OrganisationName)
            .ToList();

        return (searchResults, totalEmployerInterestsCount);
    }

    public async Task<bool> ExtendExpiry(Guid uniqueId, int numberOfDaysToExtend)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        connection.Open();

        _dynamicParametersWrapper.CreateParameters(new
        {
            uniqueId, 
            numberOfDaysToExtend
        });

        var rowsAffected = await _dbContextWrapper.ExecuteAsync(
            connection,
            "UPDATE dbo.EmployerInterest " +
            "SET ExpiryDate = DATEADD(day, @numberOfDaysToExtend, ExpiryDate), " +
            "    ModifiedOn = GETUTCDATE() " +
            "WHERE UniqueId = @uniqueId",
            _dynamicParametersWrapper.DynamicParameters);

        return rowsAffected > 0;
    }

    public async Task UpdateExtensionEmailSentDate(int id)
    {
        using var connection = _dbContextWrapper.CreateConnection();
        connection.Open();

        _dynamicParametersWrapper.CreateParameters(new
        {
            id
        });

        await _dbContextWrapper.ExecuteAsync(
            connection,
            "UPDATE dbo.EmployerInterest " +
            "SET ExtensionEmailSentDate = GETUTCDATE(), " +
            "    ModifiedOn = GETUTCDATE() " +
            "WHERE Id = @id",
            _dynamicParametersWrapper.DynamicParameters);
    }

    private async Task<int> PerformDelete(IEnumerable<int> idsToDelete, IDbConnection connection, IDbTransaction transaction)
    {
        _dynamicParametersWrapper.CreateParameters(new
        {
            employerInterestIds =
                    idsToDelete
                        .AsTableValuedParameter("dbo.IdListTableType")
        })
            .AddOutputParameter("@employerInterestsDeleted", DbType.Int32);

        await _dbContextWrapper.ExecuteAsync(
            connection,
            "DeleteEmployerInterest",
            _dynamicParametersWrapper.DynamicParameters,
            transaction,
            commandType: CommandType.StoredProcedure);

        var employerInterestsDeleted = _dynamicParametersWrapper
            .DynamicParameters
            .Get<int>("@employerInterestsDeleted");

        return employerInterestsDeleted;
    }
}
