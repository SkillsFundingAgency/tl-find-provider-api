using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using System.Net.Http.Headers;
using Sfa.Tl.Find.Provider.Application.Extensions;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration, SiteConfiguration siteConfiguration)
    {
        services
            .Configure<DfeSignInSettings>(x =>
            {
                x.ApiUri = siteConfiguration.DfeSignInSettings.ApiUri;
                x.ClientId = siteConfiguration.DfeSignInSettings.ClientId;
                x.ApiSecret = siteConfiguration.DfeSignInSettings.ApiSecret;
                x.Audience = siteConfiguration.DfeSignInSettings.Audience;
                x.Issuer = siteConfiguration.DfeSignInSettings.Issuer;
            })
            .Configure<EmailSettings>(x =>
            {
                x.GovNotifyApiKey = siteConfiguration.EmailSettings.GovNotifyApiKey;
                x.SupportEmailAddress = siteConfiguration.EmailSettings.SupportEmailAddress;
            })
            .Configure<PostcodeApiSettings>(x =>
            {
                x.BaseUri = siteConfiguration.PostcodeApiSettings.BaseUri;
            })
            .Configure<ConnectionStringSettings>(x =>
            {
                x.SqlConnectionString = siteConfiguration.SqlConnectionString;
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