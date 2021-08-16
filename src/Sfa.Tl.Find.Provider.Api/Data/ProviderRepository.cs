using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly ILogger<ProviderRepository> _logger;

        public ProviderRepository(
            IDbContextWrapper dbContextWrapper,
            ILogger<ProviderRepository> logger)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
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
                            data = locationQualificationData.AsTableValuedParameter("dbo.LocationQualificationDataTableType")
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

        public async Task<IEnumerable<Models.Provider>> Search(
            PostcodeLocation fromPostcodeLocation,
            int? qualificationId,
            int page,
            int pageSize)
        {
            using var connection = _dbContextWrapper.CreateConnection();

            //Mapping - 
            // https://dapper-tutorial.net/knowledge-base/45505157/map-lists-of-nested-objects-with-dapper--3-level-nested-object-
            // https://dapper-tutorial.net/knowledge-base/45505157/map-lists-of-nested-objects-with-dapper--3-level-nested-object-
            var providerLookup = new Dictionary<long, Models.Provider>();

            //Try QueryMultiple - example 5 from https://www.c-sharpcorner.com/UploadFile/e4e3f7/dapper-king-of-micro-orm-C-Sharp-net/
            /*
            using (var multipleResult = db.QueryMultiple(“sp_GetContact_Address”, new { id = id }, commandType: CommandType.StoredProcedure))  
            {  
               var contact = multipleResult.Read<Contact>().SingleOrDefault();  
               var Addresses = multipleResult.Read<Address>().ToList();  
               if (contact != null && Addresses != null)  
               {  
                  contact.Addresses.AddRange(Addresses);  
               }  
            }
            */
            var providers = await _dbContextWrapper
                .QueryAsync<Models.Provider, Location, Models.Provider>(
                    connection,
                    "SearchProviders",
                    map: (p, l) =>
                    {
                        if (!providerLookup.TryGetValue(p.UkPrn, out var provider))
                        {
                            providerLookup.Add(p.UkPrn, provider = p);
                        }
                        //if (provider.Locations == null)
                        //    provider.Locations = new List<Location>();
                        provider!.Locations.Add(l);
                        return provider;
                    },
                    new
                    {
                        fromLatitude = fromPostcodeLocation.Latitude,
                        fromLongitude = fromPostcodeLocation.Longitude,
                        qualificationId,
                        page,
                        pageSize
                    },
                    splitOn: "UkPrn, Postcode",
                    commandType: CommandType.StoredProcedure);

            return providers;
        }
    }
}
