using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.Services;

namespace Sfa.Tl.Find.Provider.Api
{
    public class Startup
    {
        private IConfiguration _configuration;

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

            AddHttpClients(services, postcodeApiSettings);

            services.AddTransient<IPostcodeLookupService, PostcodeLookupService>();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private IServiceCollection AddHttpClients(IServiceCollection services, PostcodeApiSettings postcodeApiSettings)
        {
            services
                .AddHttpClient<IPostcodeLookupService, PostcodeLookupService>(
                    nameof(PostcodeLookupService),
                    (serviceProvider, client) =>
                    {
                        //var postcodeApiSettings = serviceProvider
                        //    .GetRequiredService<IOptions<PostcodeApiSettings>>()
                        //    .Value;

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
                //.ConfigurePrimaryHttpMessageHandler(_ =>
                //{
                //    var handler = new HttpClientHandler();
                //    if (handler.SupportsAutomaticDecompression)
                //    {
                //        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                //    }
                //    return handler;
                //})
                .AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(new[] {
                        TimeSpan.FromMilliseconds(200),
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    }))
                ;

            return services;
        }
    }
}
