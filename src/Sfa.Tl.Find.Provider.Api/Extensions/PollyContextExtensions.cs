using Microsoft.Extensions.Logging;
using Polly;

public static class PolicyContextItems
{
    public const string Logger = "logger";
}

public static class PollyContextExtensions
{
    public static bool TryGetLogger(this Context context, out ILogger logger)
    {
        if (context.TryGetValue(PolicyContextItems.Logger, out var loggerObject) && loggerObject is ILogger theLogger)
        {
            logger = theLogger;
            return true;
        }

        logger = null;
        return false;
    }
}