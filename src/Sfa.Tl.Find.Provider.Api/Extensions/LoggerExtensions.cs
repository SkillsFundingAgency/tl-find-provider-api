using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

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

        logger.LogInformation("{repositoryName} saved {typeName} data.",
            repositoryName, typeName);

        if (includeInserted) logger.LogInformation(" Inserted {inserted} row(s).", inserted);
        if (includeUpdated) logger.LogInformation(" Updated {updated} row(s).", updated);
        if (includeDeleted) logger.LogInformation(" Deleted {deleted} row(s).", deleted);
    }
}