using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using System.Net.Http.Headers;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using StackExchange.Redis;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, SiteConfiguration siteConfiguration)
    {
        services
            .Configure<DfeSignInSettings>(x =>
            {
                x.ConfigureDfeSignInSettings(siteConfiguration);
            })
            .Configure<EmailSettings>(x =>
            {
                x.ConfigureEmailSettings(siteConfiguration);
            })
            .Configure<EmployerInterestSettings>(x =>
            {
                x.ConfigureEmployerInterestSettings(siteConfiguration);
            })
            .Configure<PostcodeApiSettings>(x =>
            {
                x.ConfigurePostcodeApiSettings(siteConfiguration);
            })
            .Configure<ProviderSettings>(x =>
            {
                x.ConfigureProviderSettings(siteConfiguration);
            })
            .Configure<SearchSettings>(x =>
            {
                x.ConfigureSearchSettings(siteConfiguration);
            })
            .Configure<ConnectionStringSettings>(x =>
            {
                x.ConfigureConnectionStringSettings(siteConfiguration);
            });

        return services;
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services
            .AddHttpClient<IPostcodeLookupService, PostcodeLookupService>(
                (serviceProvider, client) =>
                {
                    var postcodeApiSettings = serviceProvider
                        .GetRequiredService<IOptions<PostcodeApiSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(postcodeApiSettings.BaseUri!);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<PostcodeLookupService>();

        services
            .AddHttpClient<ITownDataService, TownDataService>(
                (_, client) =>
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<TownDataService>();

        services.AddHttpClient<IDfeSignInApiService, DfeSignInApiService>(
                (serviceProvider, client) =>
                {
                    var dfeSignInSettings = serviceProvider
                        .GetRequiredService<IOptions<DfeSignInSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(dfeSignInSettings.ApiUri!);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
            .AddRetryPolicyHandler<DfeSignInApiService>();

        return services;
    }

    public static IServiceCollection AddNotifyService(
        this IServiceCollection services,
        string? govNotifyApiKey)
    {
        if (!string.IsNullOrEmpty(govNotifyApiKey))
        {
            services.AddTransient<IAsyncNotificationClient>(
                _ => new NotificationClient(govNotifyApiKey));
        }

        return services;
    }

    public static IServiceCollection AddCachingServices(
        this IServiceCollection services,
        string? redisCacheConnectionString)
    {
        if (!string.IsNullOrEmpty(redisCacheConnectionString))
        {
            services
                .AddStackExchangeRedisCache(o =>
                {
                    o.Configuration = redisCacheConnectionString;
                })
                .AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(redisCacheConnectionString))
                .AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services
                .AddMemoryCache()
                .AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}
