using System;
using System.IO;
using System.Net.Http;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
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
    }
}
