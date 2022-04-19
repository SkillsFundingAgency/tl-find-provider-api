using System;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var siteConfiguration = builder.Configuration.LoadConfigurationOptions();

    builder.Services.AddApplicationInsightsTelemetry();

    builder.Services.AddConfigurationOptions(builder.Configuration, siteConfiguration);

    builder.Services.AddMemoryCache();

    builder.Services.AddApiVersioningPolicy();

    builder.Services.Configure<RouteOptions>(options =>
    {
        options.AppendTrailingSlash = true;
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });

    builder.Services.AddControllers();

    builder.Services.AddSwagger("v2",
        "T Levels Find a Provider Api",
        "v2",
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

    builder.Services
        .AddCorsPolicy(Constants.CorsPolicyName, siteConfiguration.AllowedCorsOrigins)
        .AddPolicyRegistry()
        .AddDapperRetryPolicy();

    builder.Services.AddHttpClients();

    builder.Services
        .AddScoped<IDbContextWrapper, DbContextWrapper>()
        .AddScoped<IDateTimeService, DateTimeService>()
        .AddTransient<IProviderDataService, ProviderDataService>()
        .AddTransient<ITownDataService, TownDataService>()
        .AddTransient<IProviderRepository, ProviderRepository>()
        .AddTransient<IQualificationRepository, QualificationRepository>()
        .AddTransient<IRouteRepository, RouteRepository>()
        .AddTransient<ITownRepository, TownRepository>();

    builder.Services.AddQuartzServices(
        siteConfiguration.CourseDirectoryImportSchedule,
        siteConfiguration.TownDataImportSchedule);

    builder.Services
        .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
        .AddRateLimitPolicy()
        .Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 2;
            options.RequireHeaderSymmetry = false;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

    var app = builder.Build();

    app.UseForwardedHeaders();

    app.UseSecurityHeaders(
        SecurityHeaderExtensions
            .GetHeaderPolicyCollection(app.Environment.IsDevelopment()));

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint(
            "/swagger/v2/swagger.json",
            "T Levels Find a Provider.Api v2"));
    }

    if (!string.IsNullOrWhiteSpace(siteConfiguration.AllowedCorsOrigins))
    {
        app.UseCors(Constants.CorsPolicyName);
    }

    app.UseHttpsRedirection();

    app.UseIpRateLimiting();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.Run();
}
catch (Exception ex)
{
    var message = $"Severe startup issue. {ex.Message}. \n {ex.StackTrace}";
    Console.WriteLine(message);

    var appInsightsInstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
    if (!string.IsNullOrEmpty(appInsightsInstrumentationKey))
    {
        var client = new TelemetryClient(TelemetryConfiguration.CreateDefault())
        {
            InstrumentationKey = appInsightsInstrumentationKey
        };

        client.TrackTrace("Severe startup issue.", SeverityLevel.Critical);
        client.TrackTrace(message, SeverityLevel.Critical);
    }
}