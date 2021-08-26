using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogChangeResults(this ILogger logger,
            IEnumerable<(string Change, int ChangeCount)> updateResult,
            string repositoryName,
            string typeName,
            bool includeInserted = true,
            bool includeUpdated = true,
            bool includeDeleted = true)
        {
            var (inserted, updated, deleted) = updateResult.ConvertToTuple();

            var message = $"{repositoryName} saved {typeName} data.";
            if (includeInserted) message += $" Inserted {inserted} row{(inserted == 1 ? "" : "s")}.";
            if (includeUpdated) message += $" Updated {updated} row{(updated == 1 ? "" : "s")}.";
            if (includeDeleted) message += $" Deleted {deleted} row{(deleted == 1 ? "" : "s")}.";

            logger.LogInformation(message);
        }
    }
}
