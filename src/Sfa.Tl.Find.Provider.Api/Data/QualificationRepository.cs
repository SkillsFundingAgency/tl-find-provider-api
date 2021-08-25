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
    public class QualificationRepository : IQualificationRepository
    {
        private readonly IDbContextWrapper _dbContextWrapper;
        private readonly ILogger<QualificationRepository> _logger;

        public QualificationRepository(
            IDbContextWrapper dbContextWrapper,
            ILogger<QualificationRepository> logger)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Qualification>> GetAll()
        {
            using var connection = _dbContextWrapper.CreateConnection();

            return await _dbContextWrapper.QueryAsync<Qualification>(
                connection,
                "SELECT Id, Name " +
                "FROM dbo.Qualification " +
                "WHERE IsDeleted = 0 " +
                "ORDER BY Name");
        }

        public async Task Save(IEnumerable<Qualification> qualifications)
        {
            try
            {
                using var connection = _dbContextWrapper.CreateConnection();
                connection.Open();

                using var transaction = _dbContextWrapper.BeginTransaction(connection);

                var updateResult = await _dbContextWrapper
                    .QueryAsync<(string Change, int ChangeCount)>(
                        connection,
                        "UpdateQualifications",
                        new
                        {
                            data = qualifications.AsTableValuedParameter("dbo.QualificationDataTableType")
                        },
                        transaction,
                        commandType: CommandType.StoredProcedure);

                LogChangeResults(updateResult, nameof(qualifications));

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when saving providers");
                throw;
            }
        }

        private void LogChangeResults(
            IEnumerable<(string Change, int ChangeCount)> updateResult,
            string typeName,
            bool includeInserted = true,
            bool includeUpdated = true,
            bool includeDeleted = true)
        {
            var changeResults = updateResult.ConvertToTuple();

            var message = $"{nameof(QualificationRepository)} saved {typeName} data - ";
            if (includeInserted) message += $"inserted {changeResults.Inserted}, ";
            if (includeUpdated) message += $"updated {changeResults.Updated}, ";
            if (includeDeleted) message += $"deleted {changeResults.Deleted}.";

            _logger.LogInformation(message);
        }
    }
}
