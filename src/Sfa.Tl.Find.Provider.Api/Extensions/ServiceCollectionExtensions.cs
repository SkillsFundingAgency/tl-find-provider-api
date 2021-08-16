using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;

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
            string cronSchedule,
            bool runOnStartup = false)
        {
            if (string.IsNullOrEmpty(cronSchedule) && !runOnStartup)
            {
                //Can't add an empty job unless it is persisted in database
                return services;
            }

            services.AddQuartz(q =>
            {
                // Normally would take this from appsettings.json, just show it's possible
                q.SchedulerName = "Example Quartz Scheduler";

                // Use a Scoped container for creating IJobs
                q.UseMicrosoftDependencyInjectionJobFactory();

                //q.UsePersistentStore(s =>
                //{
                //    s.UseSqlServer(connectionString);
                //    s.UseClustering();
                //    s.UseProperties = true;
                //    s.UseJsonSerializer();
                //});

                var jobKey = new JobKey("Import Course Data");
                q.AddJob<CourseDataImportJob>(opts => opts.WithIdentity(jobKey));

                if (runOnStartup)
                {
                    //TODO: Consider a start-up job that check s if there are any, and then runs the main job

                    q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .StartNow()
                );
                }

                if (!string.IsNullOrEmpty(cronSchedule))
                {
                    q.AddTrigger(opts => opts
                        .ForJob(jobKey)
                        .WithSchedule(
                            CronScheduleBuilder
                                .CronSchedule(cronSchedule)));
                }
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
