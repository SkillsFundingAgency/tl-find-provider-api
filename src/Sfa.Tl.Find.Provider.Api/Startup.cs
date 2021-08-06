using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            var postcodeApiSettings = new PostcodeApiSettings();
            var postcodeApiConfiguration = _configuration.GetSection(nameof(PostcodeApiSettings));
            postcodeApiConfiguration.Bind(postcodeApiSettings);

            services
                .AddOptions<PostcodeApiSettings>()
                .Bind(postcodeApiConfiguration);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<OptionalRouteParameterOperationFilter>();

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "T Levels Find a Provider Api",
                        Version = "v1"
                    });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            AddCorsPolicy(services);

            AddHttpClients(services);

            services.AddTransient<IProviderDataService, ProviderDataService>();
            services.AddTransient<IProviderRepository, ProviderRepository>();
            services.AddTransient<IQualificationRepository, QualificationRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sfa.Tl.Find.Provider.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            if (!string.IsNullOrWhiteSpace(_configuration["AllowedOrigins"]))
                app.UseCors(CorsPolicyName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IServiceCollection AddCorsPolicy(IServiceCollection services)
        {
            var allowedOrigins = _configuration["AllowedOrigins"];
            if(!string.IsNullOrWhiteSpace(allowedOrigins))
            {
                var corsOrigins = allowedOrigins.Split(new[] { ';', ',' });
                services.AddCors(options => options.AddPolicy(CorsPolicyName, builder =>
                    builder
                        .WithMethods(HttpMethod.Get.Method)
                        .AllowAnyHeader()
                        .WithOrigins(corsOrigins)));
            }

            return services;
        }

        private IServiceCollection AddHttpClients(IServiceCollection services)
        {
            //https://www.stevejgordon.co.uk/ihttpclientfactory-patterns-using-typed-clients-from-singleton-services
            //https://github.com/dotnet/runtime/issues/45035
            //https://www.parksq.co.uk/dotnet-core/dependency-injection-httpclient-and-ihttpclientfactory
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

                        //client.DefaultRequestHeaders.Add("Accept", "application/json");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                        //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    }
                )
                .ConfigurePrimaryHttpMessageHandler(_ =>
                {
                    var handler = new HttpClientHandler();
                    //if (handler.SupportsAutomaticDecompression)
                    //{
                    //    handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    //}
                    return handler;
                })
                .AddPolicyHandler((serviceProvider, _) =>
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(new[]
                            {
                                TimeSpan.FromMilliseconds(200),
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(5),
                                TimeSpan.FromSeconds(10)
                            },
                            (outcome, timespan, retryAttempt, context) =>
                            {
                                serviceProvider
                                    .GetService<ILogger<PostcodeLookupService>>()?
                                    .LogWarning($"Transient HTTP error in {nameof(PostcodeLookupService)}. Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}.");
                            }
                ));

            return services;
        }
    }
}
