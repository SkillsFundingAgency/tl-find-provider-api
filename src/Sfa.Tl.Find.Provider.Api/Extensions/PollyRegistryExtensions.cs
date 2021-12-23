using System;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class PollyRegistryExtensions
{
    public static IPolicyRegistry<string> AddDapperRetryPolicy(this IPolicyRegistry<string> policyRegistry)
    {
        var backoff = Backoff.DecorrelatedJitterBackoffV2(
            TimeSpan.FromSeconds(1),
            7,
            seed: 100);

        var retryPolicy = Policy
            .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .Or<SqlException>(SqlServerTransientExceptionDetector.CouldNotOpenConnection)
            .Or<TimeoutException>()
            .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .WaitAndRetryAsync(
                backoff,
                (exception,
                    sleepDuration,
                    context) =>
                {
                    if (!context.TryGetLogger(out var logger)) return;

                    if (exception != null)
                    {
                        logger.LogWarning(exception, "An error occurred when calling the database. Retrying after {sleepDuration}. Retry {retryAttempt} for {policyKey}",
                            sleepDuration, context.Count, context.PolicyKey);
                    }
                    else
                    {
                        logger.LogWarning("A non success code was received on retry {RetryAttempt} for {PolicyKey}",
                            context.Count, context.PolicyKey);
                    }
                })
            .WithPolicyKey(Constants.DapperRetryPolicyName);

        policyRegistry.Add(Constants.DapperRetryPolicyName, retryPolicy);

        return policyRegistry;
    }
}