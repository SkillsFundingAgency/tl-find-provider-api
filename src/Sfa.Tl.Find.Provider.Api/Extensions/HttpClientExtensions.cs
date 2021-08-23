using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddRetryPolicyHandler<T>(this IHttpClientBuilder httpClientBuilder)
        {
            return httpClientBuilder
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(new[]
                            {
                                TimeSpan.FromMilliseconds(200),
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(5),
                                TimeSpan.FromSeconds(10)
                            },
                            (_, timespan, retryAttempt, _) =>
                            {
                                serviceProvider
                                    .GetService<ILogger<T>>()?
                                    .LogWarning($"Transient HTTP error in {typeof(T).Name}. " +
                                                $"Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}.");
                            }
                        ));
        }
    }
}
