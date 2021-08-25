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
            var changeResults = updateResult.ConvertToTuple();

            var message = $"{repositoryName} saved {typeName} data - ";
            if (includeInserted) message += $"inserted {changeResults.Inserted}, ";
            if (includeUpdated) message += $"updated {changeResults.Updated}, ";
            if (includeDeleted) message += $"deleted {changeResults.Deleted}.";

            logger.LogInformation(message);
        }
    }
}
