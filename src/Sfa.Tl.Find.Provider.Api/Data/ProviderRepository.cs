using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data
{
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

        public async Task Save(IEnumerable<Models.Provider> providers)
        {
            try
            {
                var locationData = new List<LocationDto>();
                var locationQualificationData = new List<LocationQualificationDto>();

                foreach (var provider in providers)
                {
                    //locationData = provider.Locations.MapToLocationDtoCollection(provider.UkPrn);
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
    }
}
