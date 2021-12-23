#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class ProviderRepository : IProviderRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<ProviderRepository> _logger;

    public ProviderRepository(
        IDbContextWrapper dbContextWrapper,
        IDateTimeService dateTimeService,
        ILogger<ProviderRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> HasAny()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var result = await _dbContextWrapper.ExecuteScalarAsync<int>(
            connection,
            "SELECT COUNT(1) " +
            "WHERE EXISTS (SELECT 1 FROM dbo.Provider)");

        return result != 0;
    }

    public async Task Save(IList<Models.Provider> providers)
    {
        try
        {
            var locationData = new List<LocationDto>();
            var locationQualificationData = new List<LocationQualificationDto>();

            foreach (var provider in providers)
            {
                foreach (var location in provider.Locations)
                {
                    locationData.Add(location.MapToLocationDto(provider.UkPrn));
                    locationQualificationData.AddRange(
                        location
                            .DeliveryYears
                            .MapToLocationQualificationDtoList(provider.UkPrn, location.Postcode));
                }
            }

            using var connection = _dbContextWrapper.CreateConnection();
            connection.Open();

            using var transaction = _dbContextWrapper.BeginTransaction(connection);

            var providerUpdateResult = await _dbContextWrapper
                .QueryAsync<(string Change, int ChangeCount)>(
                    connection,
                    "UpdateProviders",
                    new
                    {
                        data = providers.AsTableValuedParameter("dbo.ProviderDataTableType")
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure);

            _logger.LogChangeResults(providerUpdateResult, nameof(ProviderRepository), nameof(providers));

            var locationUpdateResult = await _dbContextWrapper
                .QueryAsync<(string Change, int ChangeCount)>(
                    connection,
                    "UpdateLocations",
                    new
                    {
                        data = locationData.AsTableValuedParameter("dbo.LocationDataTableType")
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure);

            _logger.LogChangeResults(locationUpdateResult, nameof(ProviderRepository), "locations");

            var locationQualificationUpdateResult = await _dbContextWrapper
                .QueryAsync<(string Change, int ChangeCount)>(
                    connection,
                    "UpdateLocationQualifications",
                    new
                    {
                        data = locationQualificationData.AsTableValuedParameter(
                            "dbo.LocationQualificationDataTableType")
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure);

            _logger.LogChangeResults(locationQualificationUpdateResult, nameof(ProviderRepository),
                "location qualifications", includeUpdated: false);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when saving providers");
            throw;
        }
    }

    public async Task<IEnumerable<ProviderSearchResult>> Search(
        PostcodeLocation fromPostcodeLocation,
        int? qualificationId,
        int page,
        int pageSize)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        var providerSearchResults = new Dictionary<string, ProviderSearchResult>();

        await _dbContextWrapper
            .QueryAsync<ProviderSearchResult, DeliveryYear, Qualification, ProviderSearchResult>(
                connection,
                "SearchProviders",
                map: (p, ly, q) =>
                {
                    var key = $"{p.UkPrn}_{p.Postcode}";
                    if (!providerSearchResults.TryGetValue(key, out var searchResult))
                    {
                        //TODO: Remove code here
                        var isRunningFromTest = AppDomain.CurrentDomain.GetAssemblies().Any(
                                a => a.FullName!.ToLowerInvariant().StartsWith("xunit.runner"));
                        if (!isRunningFromTest)
                        {
                            var r = __random.Next(100);
                            switch (r)
                            {
                                case 1:
                                    throw SqlExceptionFactory.Create(49920);
                                case 99:
                                    throw SqlExceptionFactory.Create(40613);
                            }
                        }
                        //End of temp code block

                        providerSearchResults.Add(key, searchResult = p);
                        searchResult.JourneyToLink = fromPostcodeLocation.CreateJourneyLink(searchResult.Postcode);
                    }

                    //TODO: Consider a dictionary, and lookup like above
                    // - the year list is small so linear search would be reasonably fast

                    var deliveryYear = searchResult
                        .DeliveryYears
                        .FirstOrDefault(y => y.Year == ly.Year);

                    if (deliveryYear == null)
                    {
                        deliveryYear = ly;

                        deliveryYear.IsAvailableNow = deliveryYear.Year.IsAvailableAtDate(_dateTimeService.Today);

                        searchResult.DeliveryYears.Add(deliveryYear);
                    }

                    if (deliveryYear.Qualifications.All(z => z.Id != q.Id))
                    {
                        deliveryYear.Qualifications.Add(q);
                    }

                    return searchResult;
                },
                new
                {
                    fromLatitude = fromPostcodeLocation.Latitude,
                    fromLongitude = fromPostcodeLocation.Longitude,
                    qualificationId,
                    page,
                    pageSize
                },
                splitOn: "UkPrn, Postcode, Year, Id",
                commandType: CommandType.StoredProcedure);

        return providerSearchResults
            .Values
            .OrderBy(s => s.Distance)
            .ThenBy(s => s.ProviderName)
            .ThenBy(s => s.LocationName)
            .ToList();
    }

    private static readonly Random __random = new();

    private static class SqlExceptionFactory
    {
        public static SqlException Create(int number)
        {
            Exception? innerEx = null;
            var c = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var errors = (c[0].Invoke(null) as SqlErrorCollection)!;
            var errorList = (errors.GetType().GetField("_errors", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(errors) as List<object>)!;
            c = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var nineC = c.FirstOrDefault(f => f.GetParameters().Length == 9)!;
            var sqlError = (nineC.Invoke(new object?[] { number, (byte)0, (byte)0, "", "", "", 0, (uint)0, innerEx }) as SqlError)!;
            errorList.Add(sqlError);

            return (Activator.CreateInstance(typeof(SqlException), BindingFlags.NonPublic | BindingFlags.Instance, null, new object?[] { "test", errors,
                innerEx, Guid.NewGuid() }, null) as SqlException)!;
        }
    }
}