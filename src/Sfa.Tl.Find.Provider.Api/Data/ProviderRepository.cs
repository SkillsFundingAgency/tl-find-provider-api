using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly IDbContextWrapper _dbContextWrapper;

        public ProviderRepository(IDbContextWrapper dbContextWrapper)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        }

        public async Task<(int Inserted, int Updated, int Deleted)> Save(IEnumerable<Models.Provider> providers)
        {
            using var connection = _dbContextWrapper.CreateConnection();
            //using var transaction = connection.BeginTransaction();

            var updateResult = await _dbContextWrapper
                .QueryAsync<(string Change, int ChangeCount)>(
                    connection,
                    "UpdateProviders",
                    new
                    {
                        data = providers.AsTableValuedParameter("dbo.ProviderDataTableType")
                    },
                    commandType: CommandType.StoredProcedure);

            //transaction.Commit();

            return updateResult.ConvertToTuple();
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
