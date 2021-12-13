using System;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class PollyRegistryExtensions
{
    private const int RetryCount = 4;
    private static readonly Random Random = new();

    public static IPolicyRegistry<string> AddDapperRetryPolicy(this IPolicyRegistry<string> policyRegistry)
    {
        var retryPolicy = Policy
            .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .Or<SqlException>(SqlServerTransientExceptionDetector.CouldNotOpenConnection)
            .Or<TimeoutException>()
            .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .WaitAndRetryAsync(
                RetryCount,
                currentRetryNumber => 
                    TimeSpan.FromSeconds(Math.Pow(1.5, currentRetryNumber - 1)) 
                    + TimeSpan.FromMilliseconds(Random.Next(0, 100)),
                (exception,
                    sleepDuration,
                    retryNumber,
                    context) =>
                {
                    if (!context.TryGetLogger(out var logger)) return;
                        
                    if (exception != null)
                    {
                        logger.LogWarning(exception, "An error occurred when calling the database. Retrying after {sleepDuration}. Retry {retryAttempt} for {policyKey}",
                            sleepDuration, retryNumber, context.PolicyKey);
                    }
                    else
                    {
                        logger.LogWarning("A non success code was received on retry {RetryAttempt} for {PolicyKey}",
                            retryNumber, context.PolicyKey);
                    }
                })
            .WithPolicyKey(Constants.DapperRetryPolicyName);

        policyRegistry.Add(Constants.DapperRetryPolicyName, retryPolicy);

        return policyRegistry;
    }
}