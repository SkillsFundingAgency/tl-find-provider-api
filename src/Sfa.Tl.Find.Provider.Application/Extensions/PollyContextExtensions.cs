using Microsoft.Extensions.Logging;
using Polly;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class PollyContextExtensions
{
    public static bool TryGetLogger(this Context context, out ILogger logger)
    {
        if (context.
                TryGetValue(PolicyContextItems.Logger, out var loggerObject) 
            && loggerObject is ILogger actualLogger)
        {
            logger = actualLogger;
            return true;
        }

        logger = null;
        return false;
    }
}