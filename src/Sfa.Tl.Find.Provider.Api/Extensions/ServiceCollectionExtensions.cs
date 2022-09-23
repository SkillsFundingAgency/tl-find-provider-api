using System.Net;
using System.Net.Http.Headers;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Notify.Client;
using Notify.Interfaces;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiVersioningPolicy(
        this IServiceCollection services)
    {
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(
                Constants.DefaultApiMajorVersion, 
                Constants.DefaultApiMinorVersion);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        return services;
    }

    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration, SiteConfiguration siteConfiguration)
    {
        var rateLimiting = configuration?.GetSection("IpRateLimiting");
        if (rateLimiting != null)
        {
            services
                .Configure<IpRateLimitOptions>(rateLimiting);
        }

        services
            .Configure<ApiSettings>(x =>
            {
                x.AppId = siteConfiguration.ApiSettings.AppId;
                x.ApiKey = siteConfiguration.ApiSettings.ApiKey;
            })
            .Configure<CourseDirectoryApiSettings>(x =>
            {
                x.BaseUri = siteConfiguration.CourseDirectoryApiSettings.BaseUri;
                x.ApiKey = siteConfiguration.CourseDirectoryApiSettings.ApiKey;
            })
            .Configure<EmailSettings>(x =>
            {
                x.GovNotifyApiKey = siteConfiguration.EmailSettings.GovNotifyApiKey;
                x.DeliveryStatusToken = siteConfiguration.EmailSettings.DeliveryStatusToken;
                x.SupportEmailAddress = siteConfiguration.EmailSettings.SupportEmailAddress;
            })
            .Configure<EmployerInterestSettings>(x =>
            {
                x.RetentionDays = siteConfiguration.EmployerInterestSettings.RetentionDays;
                x.CleanupJobSchedule = siteConfiguration.EmployerInterestSettings.CleanupJobSchedule;
            })
            .Configure<GoogleMapsApiSettings>(x =>
            {
                x.ApiKey = siteConfiguration.GoogleMapsApiSettings.ApiKey;
                x.BaseUri = siteConfiguration.GoogleMapsApiSettings.BaseUri;
            })
            .Configure<PostcodeApiSettings>(x =>
            {
                x.BaseUri = siteConfiguration.PostcodeApiSettings.BaseUri;
            })
            .Configure<SearchSettings>(x =>
            {
                x.MergeAdditionalProviderData = siteConfiguration.SearchSettings?.MergeAdditionalProviderData ?? false;
            })
            .Configure<ConnectionStringSettings>(x =>
            {
                x.SqlConnectionString = siteConfiguration.SqlConnectionString;
            });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        string policyName,
        string allowedOrigins)
    {
        if (!string.IsNullOrWhiteSpace(allowedOrigins))
        {
            var corsOrigins = allowedOrigins
                .Split(';', ',')
                .Select(s => s.TrimEnd('/'))
                .ToArray();

            services.AddCors(options => options.AddPolicy(policyName, builder =>
                builder
                    .WithMethods(HttpMethod.Get.Method)
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

                    client.BaseAddress =
                        postcodeApiSettings.BaseUri.EndsWith("/")
                            ? new Uri(postcodeApiSettings.BaseUri)
                            : new Uri(postcodeApiSettings.BaseUri + "/");

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<PostcodeLookupService>();

        services
            .AddHttpClient<ICourseDirectoryService, CourseDirectoryService>(
                (serviceProvider, client) =>
                {
                    var courseDirectoryApiSettings = serviceProvider
                        .GetRequiredService<IOptions<CourseDirectoryApiSettings>>()
                        .Value;

                    client.BaseAddress =
                        courseDirectoryApiSettings.BaseUri.EndsWith("/")
                            ? new Uri(courseDirectoryApiSettings.BaseUri)
                            : new Uri(courseDirectoryApiSettings.BaseUri + "/");

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", courseDirectoryApiSettings.ApiKey);

                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                })
            .ConfigurePrimaryHttpMessageHandler(_ =>
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }
                return handler;
            })
            .AddRetryPolicyHandler<CourseDirectoryService>();

        services
            .AddHttpClient<IGoogleMapsApiService, GoogleMapsApiService>(
                (serviceProvider, client) =>
                {
                    var googleMapsApiSettings = serviceProvider
                        .GetRequiredService<IOptions<GoogleMapsApiSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(googleMapsApiSettings.BaseUri);

                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<GoogleMapsApiService>();

        services
            .AddHttpClient<ITownDataService, TownDataService>(
                (_, client) =>
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }
            )
            .AddRetryPolicyHandler<TownDataService>();

        return services;
    }

    public static IServiceCollection AddNotifyService(
        this IServiceCollection services,
        string govNotifyApiKey)
    {
        if (!string.IsNullOrEmpty(govNotifyApiKey))
        {
            services.AddTransient<IAsyncNotificationClient, NotificationClient>(
                _ => new NotificationClient(govNotifyApiKey));
        }

        return services;
    }

    public static IServiceCollection AddQuartzServices(
        this IServiceCollection services,
        string courseDirectoryImportCronSchedule = null,
        string townDataImportCronSchedule = null,
        string employerInterestCleanupCronSchedule = null)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = "Find a Provider Quartz Scheduler";

            q.UseMicrosoftDependencyInjectionJobFactory();

            var startupJobKey = new JobKey(Constants.StartupTasksJobKeyName);
            q.AddJob<InitializationJob>(opts => 
                    opts.WithIdentity(startupJobKey))
                .AddTrigger(opts => opts
                    .ForJob(startupJobKey)
                    .StartNow());

            if (!string.IsNullOrEmpty(courseDirectoryImportCronSchedule))
            {
                var courseImportJobKey = new JobKey(Constants.CourseDirectoryImportJobKeyName);
                q.AddJob<CourseDataImportJob>(opts => 
                        opts.WithIdentity(courseImportJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(courseImportJobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(courseDirectoryImportCronSchedule)));
            }
            
            if (!string.IsNullOrEmpty(townDataImportCronSchedule))
            {
                var townImportJobKey = new JobKey(Constants.ImportTownDataJobKeyName);
                q.AddJob<TownDataImportJob>(opts => 
                        opts.WithIdentity(townImportJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(townImportJobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(townDataImportCronSchedule)));
            }

            if (!string.IsNullOrEmpty(employerInterestCleanupCronSchedule))
            {
                var employerInterestCleanupJobKey = new JobKey(Constants.EmployerInterestCleanupJobKeyName);
                q.AddJob<EmployerInterestCleanupJob>(opts => 
                        opts.WithIdentity(employerInterestCleanupJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(employerInterestCleanupJobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(employerInterestCleanupCronSchedule)));
            }
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
    
    public static IServiceCollection AddRateLimitPolicy(
        this IServiceCollection services)
    {
        services.AddInMemoryRateLimiting();

        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IServiceCollection AddSwagger(
        this IServiceCollection services,
        string name,
        string title,
        string version,
        string xmlFile = null)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(name,
                new OpenApiInfo
                {
                    Title = title,
                    Version = version
                });

            c.ResolveConflictingActions(apiDescriptions 
                => apiDescriptions.First());

            //c.EnableAnnotations(); //Needed with Swashbuckle.AspNetCore.Annotations

            if (xmlFile != null)
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}