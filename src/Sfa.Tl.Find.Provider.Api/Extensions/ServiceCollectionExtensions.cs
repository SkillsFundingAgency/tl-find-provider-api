﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.Services;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            string policyName,
            string allowedOrigins)
        {
            if (!string.IsNullOrWhiteSpace(allowedOrigins))
            {
                var splitterChars = new[] { ';', ',' };
                var corsOrigins = allowedOrigins.Split(splitterChars);
                services.AddCors(options => options.AddPolicy(policyName, builder =>
                    builder
                        .WithMethods(HttpMethod.Get.Method)
                        .AllowAnyHeader()
                        .WithOrigins(corsOrigins)));
            }

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

                if (xmlFile != null)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static IServiceCollection AddHostedQuartzServices(
            this IServiceCollection services,
            string cronSchedule)
        {
            services.AddQuartz(q =>
            {
                // Normally would take this from appsettings.json, just show it's possible
                q.SchedulerName = "Example Quartz Scheduler";

                // Use a Scoped container for creating IJobs
                q.UseMicrosoftDependencyInjectionJobFactory();

                var startupJobKey = new JobKey("Perform Startup Tasks");
                q.AddJob<InitializationJob>(opts => opts.WithIdentity(startupJobKey))
                    .AddTrigger(opts => opts
                        .ForJob(startupJobKey)
                        .StartNow());

                if (!string.IsNullOrEmpty(cronSchedule))
                {
                    var importJobKey = new JobKey("Import Course Data");
                    q.AddJob<CourseDataImportJob>(opts => opts.WithIdentity(importJobKey))
                        .AddTrigger(opts => opts
                            .ForJob(importJobKey)
                            .WithSchedule(
                                CronScheduleBuilder
                                    .CronSchedule(cronSchedule)));
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

        public static IServiceCollection AddApiVersioningPolicy(
            this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            return services;
        }
        
        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration, SiteConfiguration siteConfiguration)
        {
            services
                .Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"))
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
                .Configure<PostcodeApiSettings>(x =>
                {
                    x.BaseUri = siteConfiguration.PostcodeApiSettings.BaseUri;
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

            return services;
        }
    }
}
