using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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

        public async Task<(int Inserted, int Updated, int Deleted)> Save(IEnumerable<Models.Provider> providers)
        {
            try
            {
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

                var providerUpdates = providerUpdateResult.ConvertToTuple();
                _logger.LogInformation(
                    "ProviderRepository Saved providers - " +
                    $"inserted {providerUpdates.Inserted}, " +
                    $"updated {providerUpdates.Updated}, " +
                    $"deleted {providerUpdates.Deleted}.");

                //TODO: Move to a method/mapping extension
                var locationData = new List<LocationDto>();
                var locationQualificationData = new List<LocationQualificationDto>();
                foreach (var provider in providers)
                {
                    foreach (var location in provider.Locations)
                    {
                        locationData.Add(new LocationDto
                        {
                            UkPrn = provider.UkPrn,
                            Postcode = location.Postcode,
                            Name = location.Name,
                            AddressLine1 = location.AddressLine1,
                            AddressLine2 = location.AddressLine2,
                            Town = location.Town,
                            County = location.County,
                            Email = location.Email,
                            Telephone = location.Telephone,
                            Website = location.Website,
                            Latitude = location.Latitude,
                            Longitude = location.Longitude
                        });

                        foreach (var deliveryYear in location.DeliveryYears)
                        {
                            foreach (var qualification in deliveryYear.Qualifications)
                            {
                                locationQualificationData.Add(new LocationQualificationDto
                                {
                                    UkPrn = provider.UkPrn,
                                    Postcode = location.Postcode,
                                    DeliveryYear = deliveryYear.Year,
                                    QualificationId = qualification.Id
                                });
                            }
                        }
                    }
                }

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

                var locationUpdates = locationUpdateResult.ConvertToTuple();
                _logger.LogInformation(
                    "ProviderRepository Saved locations - " +
                    $"inserted {locationUpdates.Inserted}, " +
                    $"updated {locationUpdates.Updated}, " +
                    $"deleted {locationUpdates.Deleted}.");

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

                var locationQualificationUpdates = locationQualificationUpdateResult.ConvertToTuple();
                _logger.LogInformation(
                    "ProviderRepository Saved location qualifications - " +
                    $"inserted {locationQualificationUpdates.Inserted}, " +
                    $"deleted {locationQualificationUpdates.Deleted}.");

                transaction.Commit();

                return providerUpdates;
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
                            searchResult.JourneyToLink =
                                "https://www.google.com/maps/dir/?api=1&" +
                                $"origin={WebUtility.UrlEncode(fromPostcodeLocation.Postcode)}" +
                                $"&destination={WebUtility.UrlEncode(searchResult.Postcode)}" +
                                "&travelmode=transit";
                        }

                        //TODO: Consider a dictionary, and lookup like above
                        // - the year list is small so linear search would be reasonably fast

                        var deliveryYear = searchResult
                            .DeliveryYears
                            .FirstOrDefault(y => y.Year == ly.Year);

                        if (deliveryYear == null)
                        {
                            deliveryYear = ly;

                            deliveryYear.IsAvailableNow =
                                deliveryYear.Year < _dateTimeService.Today.Year
                                || (deliveryYear.Year == _dateTimeService.Today.Year && _dateTimeService.Today.Month < 9);
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
