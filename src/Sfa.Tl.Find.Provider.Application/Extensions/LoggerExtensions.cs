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

        var details = $"{(includeInserted ? $" Inserted { inserted} row(s)." : null)}" +
                       $"{(includeUpdated ? $" Updated {updated} row(s)." : null)}" +
                       $"{(includeDeleted ? $" Deleted {deleted} row(s)." : null)}";

        logger.LogInformation("{repositoryName} saved {typeName} data.{details}",
            repositoryName, typeName, details);
    }
}