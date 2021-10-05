using System;
using System.IO;
using System.Net.Http;
using AspNetCoreRateLimit;
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
                var corsOrigins = allowedOrigins.Split(';', ',');
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
            this IServiceCollection services,
            double requestsPerMinuteLimit = 2,
            double requestsPerHourLimit = 10000)
        {
            //services.Configure(Configuration.GetSection("IpRateLimiting"));

            // inject counter and rules stores
            services.AddInMemoryRateLimiting();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            /*
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.Configure<ClientRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new()
                    {
                        Endpoint = "*",
                        Period = "1m",
                        Limit = requestsPerMinuteLimit
                    },
                    new()
                    {
                        Endpoint = "*",
                        Period = "1h",
                        Limit = requestsPerHourLimit
                    }
                };
            });

            //https://www.cloudsavvyit.com/12306/how-to-rate-limit-requests-in-blazor-asp-net-core/
            //services.Configure<ClientRateLimitPolicies>(options =>
            //{
            //    options.
            //});

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            */

            return services;
        }
    }
}
