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
                    retryAttempt,
                    context) =>
                {
                    if (!context.TryGetLogger(out var logger)) return;

                    if (exception != null)
                    {
                        logger.LogWarning(exception, "A database error occurred on attempt {retryAttempt}. Retrying after {sleepDuration:F2}s. Policy key {policyKey}",
                            retryAttempt, sleepDuration.TotalSeconds, context.PolicyKey);
                    }
                    else
                    {
                        logger.LogWarning("Attempt {retryAttempt} for {policyKey} failed, but no exception was seen.",
                            retryAttempt, context.PolicyKey);
                    }
                })
            .WithPolicyKey(Constants.DapperRetryPolicyName);

        policyRegistry.Add(Constants.DapperRetryPolicyName, retryPolicy);

        return policyRegistry;
    }
}