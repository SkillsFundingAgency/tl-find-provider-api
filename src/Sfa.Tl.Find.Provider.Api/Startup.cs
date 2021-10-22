using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly SiteConfiguration _siteConfiguration;
        
        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _siteConfiguration = _configuration.LoadConfigurationOptions();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            AddConfigurationOptions(services);

            services.AddMemoryCache();

            services.AddApiVersioningPolicy();

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddControllers();

            services.AddSwagger("v1",
                "T Levels Find a Provider Api",
                "v1",
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            services.AddCorsPolicy(CorsPolicyName, _siteConfiguration.AllowedCorsOrigins);

            AddHttpClients(services);

            services
                .AddScoped<IDbContextWrapper>(_ =>
                    new DbContextWrapper(_siteConfiguration.SqlConnectionString))
                .AddScoped<IDateTimeService, DateTimeService>()
                .AddTransient<IProviderDataService, ProviderDataService>()
                .AddTransient<IProviderRepository, ProviderRepository>()
                .AddTransient<IQualificationRepository, QualificationRepository>()
                .AddTransient<IRouteRepository, RouteRepository>();

            services.AddHostedQuartzServices(_siteConfiguration.CourseDirectoryImportSchedule);

            services.AddRateLimitPolicy();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSecurityHeaders(
                SecurityHeaderExtensions
                    .GetHeaderPolicyCollection(env.IsDevelopment()));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "T Levels Find a Provider.Api v1"));
            }

            if (!string.IsNullOrWhiteSpace(_siteConfiguration.AllowedCorsOrigins))
            {
                app.UseCors(CorsPolicyName);
            }

            app.UseHttpsRedirection();

            app.UseIpRateLimiting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                //RequireHeaderSymmetry = true
                //ForwardLimit = 2
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IServiceCollection AddConfigurationOptions(IServiceCollection services)
        {
            services
                .Configure<IpRateLimitOptions>(_configuration.GetSection("IpRateLimiting"))
                .Configure<ApiSettings>(x =>
                {
                    x.AppId = _siteConfiguration.ApiSettings.AppId;
                    x.ApiKey = _siteConfiguration.ApiSettings.ApiKey;
                })
                .Configure<CourseDirectoryApiSettings>(x =>
                {
                    x.BaseUri = _siteConfiguration.CourseDirectoryApiSettings.BaseUri;
                    x.ApiKey = _siteConfiguration.CourseDirectoryApiSettings.ApiKey;
                })
                .Configure<PostcodeApiSettings>(x =>
                {
                    x.BaseUri = _siteConfiguration.PostcodeApiSettings.BaseUri;
                });

            return services;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private IServiceCollection AddHttpClients(IServiceCollection services)
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
