using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Notify.Exceptions;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Models;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class PollyRegistryExtensions
{
    public static IPolicyRegistry<string> AddDapperRetryPolicy(this IPolicyRegistry<string> policyRegistry)
    {
        var backoff = Backoff.ExponentialBackoff(
            TimeSpan.FromSeconds(1.2),
            7);

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

    public static IPolicyRegistry<string> AddGovNotifyRetryPolicy(this IPolicyRegistry<string> policyRegistry)
    {
        var backoff = Backoff.ExponentialBackoff(
            TimeSpan.FromSeconds(1.2),
            7);

        var retryPolicy = Policy
            .Handle<AggregateException>(e => 
                e.InnerExceptions.Any(i =>
                    i is HttpRequestException ||
                    i is TimeoutException ||
                    i is NotifyClientException))
            .Or<HttpRequestException>()
            .Or<NotifyClientException>()
            .Or<TimeoutException>()
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
                        logger.LogWarning(exception, "An error occurred on attempt {retryAttempt}. Retrying after {sleepDuration:F2}s. Policy key {policyKey}",
                            retryAttempt, sleepDuration.TotalSeconds, context.PolicyKey);
                    }
                    else
                    {
                        logger.LogWarning("Attempt {retryAttempt} for {policyKey} failed, but no exception was seen.",
                            retryAttempt, context.PolicyKey);
                    }
                })
            .WithPolicyKey(Constants.GovNotifyRetryPolicyName);

        policyRegistry.Add(Constants.GovNotifyRetryPolicyName, retryPolicy);

        return policyRegistry;
    }

    public static (IAsyncPolicy, Context) GetDapperRetryPolicy(
        this IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger logger) => 
        GetRetryPolicy(
            policyRegistry, 
            Constants.DapperRetryPolicyName, 
            logger);

    public static (IAsyncPolicy, Context) GetNotifyRetryPolicy(
        this IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger logger) =>
        GetRetryPolicy(
            policyRegistry,  
            Constants.GovNotifyRetryPolicyName, 
            logger);

    public static (IAsyncPolicy, Context) GetRetryPolicy(
        this IReadOnlyPolicyRegistry<string> policyRegistry,
        string policyKey,
        ILogger logger)
    {
        var retryPolicy =
            policyRegistry
                .Get<IAsyncPolicy>(policyKey)
            ?? Policy.NoOpAsync();

        //https://www.stevejgordon.co.uk/passing-an-ilogger-to-polly-policies
        var context = new Context($"{Guid.NewGuid()}",
            new Dictionary<string, object>
            {
                {
                    PolicyContextItems.Logger, logger
                }
            });

        return (retryPolicy, context);
    }
}