using Microsoft.Extensions.Logging;
using Polly;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class PolicyContextItems
{
    public const string Logger = "logger";
}

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