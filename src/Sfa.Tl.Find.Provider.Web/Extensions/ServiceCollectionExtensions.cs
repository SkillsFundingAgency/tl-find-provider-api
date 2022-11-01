using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using System.Net.Http.Headers;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;

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
            .Configure<ConnectionStringSettings>(x =>
            {
                x.ConfigureConnectionStringSettings(siteConfiguration);
            });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        string policyName,
        string allowedOrigins,
        params string[]? allowedMethods)
    {
        if (!string.IsNullOrWhiteSpace(allowedOrigins) && 
            allowedMethods != null && 
            allowedMethods.Any())
        {
            var corsOrigins = allowedOrigins
                .Split(';', ',')
                .Select(s => s.TrimEnd('/'))
                .ToArray();

            services.AddCors(options =>
                options
                    .AddPolicy(policyName, builder =>
                        builder
                            .WithMethods(allowedMethods)
                            .AllowAnyHeader()
                            .WithOrigins(corsOrigins)));
        }

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

                    client.BaseAddress = new Uri(postcodeApiSettings.BaseUri);
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

                    client.BaseAddress = new Uri(dfeSignInSettings.ApiUri);
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
            services.AddTransient<IAsyncNotificationClient, NotificationClient>(
                _ => new NotificationClient(govNotifyApiKey));
        }

        return services;
    }
}