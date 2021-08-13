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

                using var transaction = connection.BeginTransaction();

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
                    $"ProviderRepository Saved providers - inserted {providerUpdates.Inserted}, updated {providerUpdates.Updated}, deleted {providerUpdates.Deleted}.");

                var locationData = new List<LocationDto>();
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
                    }
                }

                var testData = locationData.AsTableValuedParameter("dbo.LocationDataTableType");

                var locationUpdateResult = await _dbContextWrapper
                    .QueryAsync<(string Change, int ChangeCount)>(
                        connection,
                        "UpdateLocations",
                        new
                        {
                            data = testData//locationData.AsTableValuedParameter("dbo.LocationDataTableType")
                        },
                        transaction,
                        commandType: CommandType.StoredProcedure);

                var locationUpdates = locationUpdateResult.ConvertToTuple();
                _logger.LogInformation(
                    $"ProviderRepository Saved locations - inserted {locationUpdates.Inserted}, updated {locationUpdates.Updated}, deleted {locationUpdates.Deleted}.");

                transaction.Commit();

                return providerUpdates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred when saving providers");
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

            var providers = await _dbContextWrapper
                .QueryAsync<Models.Provider>(
                    connection,
                    "SearchProviders",
                    new
                    {
                        fromLatitude = fromPostcodeLocation.Latitude,
                        fromLongitude = fromPostcodeLocation.Longitude,
                        qualificationId,
                        page,
                        pageSize
                    },
                    commandType: CommandType.StoredProcedure);

            return providers;
        }
    }
}
