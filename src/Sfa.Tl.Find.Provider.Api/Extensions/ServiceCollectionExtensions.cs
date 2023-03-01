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
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

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
                x.ConfigureApiSettings(siteConfiguration);
            })
            .Configure<CourseDirectoryApiSettings>(x =>
            {
                x.ConfigureCourseDirectoryApiSettings(siteConfiguration);
            })
            .Configure<EmailSettings>(x =>
            {
                x.ConfigureEmailSettings(siteConfiguration);
            })
            .Configure<EmployerInterestSettings>(x =>
            {
                x.ConfigureEmployerInterestSettings(siteConfiguration);
            })
            .Configure<GoogleMapsApiSettings>(x =>
            {
                x.ConfigureGoogleMapsApiSettings(siteConfiguration);
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

            services.AddCors(options =>
                options
                    .AddPolicy(policyName, builder =>
                        builder
                            .WithMethods(
                                HttpMethod.Get.Method,
                                HttpMethod.Post.Method,
                                HttpMethod.Delete.Method)
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
            .AddHttpClient<ICourseDirectoryService, CourseDirectoryService>(
                (serviceProvider, client) =>
                {
                    var courseDirectoryApiSettings = serviceProvider
                        .GetRequiredService<IOptions<CourseDirectoryApiSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(courseDirectoryApiSettings.BaseUri);
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
            services.AddTransient<IAsyncNotificationClient>(
                _ => new NotificationClient(govNotifyApiKey));
        }

        return services;
    }

    public static IServiceCollection AddQuartzServices(
        this IServiceCollection services,
        string sqlConnectionString,
        string courseDirectoryImportCronSchedule = null,
        string townDataImportCronSchedule = null,
        string employerInterestCleanupCronSchedule = null,
        string providerNotificationEmailImmediateCronSchedule = null,
        string providerNotificationEmailDailyCronSchedule = null,
        string providerNotificationEmailWeeklyCronSchedule = null)
    {
        services.AddQuartz(q =>
        {
            q.SchedulerName = "Find a Provider Quartz Scheduler";

            q.UseMicrosoftDependencyInjectionJobFactory();

            q.UsePersistentStore(x =>
            {
                x.UseProperties = true;
                x.UseClustering();
                x.UseSqlServer(sqlConnectionString);
                x.UseJsonSerializer();
            });

            //var startupJobKey = new JobKey(JobKeys.StartupTasks);
            //q.AddJob<InitializationJob>(opts =>
            //        opts.WithIdentity(startupJobKey))
            //    .AddTrigger(opts => opts
            //        .ForJob(startupJobKey)
            //        .StartNow());

            if (!string.IsNullOrEmpty(courseDirectoryImportCronSchedule))
            {
                var courseDataImportJobKey = new JobKey(JobKeys.CourseDirectoryImport);
                q.AddJob<CourseDataImportJob>(opts =>
                        opts.WithIdentity(courseDataImportJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(courseDataImportJobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(courseDirectoryImportCronSchedule)));
            }

            // Removed because the ONS API now requires a key. Use the manual file upload instead
            //if (!string.IsNullOrEmpty(townDataImportCronSchedule))
            //{
            //    var townDataImportJobKey = new JobKey(JobKeys.ImportTownData);
            //    q.AddJob<TownDataImportJob>(opts =>
            //            opts.WithIdentity(townDataImportJobKey))
            //        .AddTrigger(opts => opts
            //            .ForJob(townDataImportJobKey)
            //            .WithSchedule(
            //                CronScheduleBuilder
            //                    .CronSchedule(townDataImportCronSchedule)));
            //}

            if (!string.IsNullOrEmpty(employerInterestCleanupCronSchedule))
            {
                var employerInterestCleanupJobKey = new JobKey(JobKeys.EmployerInterestCleanup);
                q.AddJob<EmployerInterestCleanupJob>(opts =>
                        opts.WithIdentity(employerInterestCleanupJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(employerInterestCleanupJobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(employerInterestCleanupCronSchedule)));
            }

            if (!string.IsNullOrEmpty(providerNotificationEmailImmediateCronSchedule))
            {
                var jobKey = new JobKey(JobKeys.ProviderNotificationEmailImmediate);
                q.AddJob<ProviderNotificationEmailJob>(opts =>
                    {
                        opts.WithIdentity(jobKey);
                        opts.UsingJobData(JobDataKeys.NotificationFrequency,
                            NotificationFrequency.Immediately.ToString());
                    })
                    .AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(providerNotificationEmailImmediateCronSchedule)));
            }

            if (!string.IsNullOrEmpty(providerNotificationEmailDailyCronSchedule))
            {
                var jobKey = new JobKey(JobKeys.ProviderNotificationEmailDaily);
                q.AddJob<ProviderNotificationEmailJob>(opts =>
                    {
                        opts.WithIdentity(jobKey);
                        opts.UsingJobData(JobDataKeys.NotificationFrequency,
                            NotificationFrequency.Daily.ToString());
                    })
                    .AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(providerNotificationEmailDailyCronSchedule)));
            }

            if (!string.IsNullOrEmpty(providerNotificationEmailWeeklyCronSchedule))
            {
                var jobKey = new JobKey(JobKeys.ProviderNotificationEmailWeekly);
                q.AddJob<ProviderNotificationEmailJob>(opts =>
                    {
                        opts.WithIdentity(jobKey);
                        opts.UsingJobData(JobDataKeys.NotificationFrequency,
                            NotificationFrequency.Weekly.ToString());
                    })
                    .AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(providerNotificationEmailWeeklyCronSchedule)));
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