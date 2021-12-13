using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class PolicyNames
    {
        public const string BasicRetry = "basic-retry";
        public const string DapperRetry = "dapper-transient-error-retry";
    }


    public static class PollyRegistryExtensions
    {

        private const int RetryCount = 4;
        private static readonly Random Random = new();
        private static readonly IEnumerable<TimeSpan> RetryTimes = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        };

        public static IPolicyRegistry<string> AddDapperRetryPolicy(this IPolicyRegistry<string> policyRegistry)
        {
            var retryPolicy = Policy
                .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
                .Or<TimeoutException>()
                .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
                .WaitAndRetryAsync(
                    RetryCount,
                    currentRetryNumber => TimeSpan.FromSeconds(Math.Pow(1.5, currentRetryNumber - 1)) + TimeSpan.FromMilliseconds(Random.Next(0, 100)),
                    (exception,
                        sleepDuration,
                        retryNumber,
                        context) =>
                    {
                        if (!context.TryGetLogger(out var logger)) return;
                        
#if DEBUG
                        Debug.WriteLine($"=== Attempt {retryNumber} ===");
                        Debug.WriteLine($"{nameof(exception)}: {exception}");
                        Debug.WriteLine($"{nameof(context)}: {context}");
                        Debug.WriteLine($"{nameof(sleepDuration)}: {sleepDuration}");
#endif
                        if (exception != null)
                        {
                            logger.LogError(exception, "An exception occurred on retry {RetryAttempt} for {PolicyKey}", 
                                retryNumber, context.PolicyKey);
                        }
                        else
                        {
                            logger.LogError("A non success code was received on retry {RetryAttempt} for {PolicyKey}",
                                retryNumber, context.PolicyKey);
                        }
                    })
                .WithPolicyKey(PolicyNames.DapperRetry);

            policyRegistry.Add(PolicyNames.DapperRetry, retryPolicy);

            return policyRegistry;
        }
    }
}
