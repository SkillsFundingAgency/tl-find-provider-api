using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.HealthChecks;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Providers;

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

    builder.Services.AddSwagger("v3",
        "T Levels Find a Provider Api",
        "v3",
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

    builder.Services
        .AddCorsPolicy(Constants.CorsPolicyName, siteConfiguration.AllowedCorsOrigins);

    builder.Services
        .AddPolicyRegistry()
        .AddDapperRetryPolicy()
        .AddGovNotifyRetryPolicy();

    builder.Services.AddHttpClients();

    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = int.MaxValue;
    });

    builder.Services.Configure<FormOptions>(o =>
    {
        o.ValueLengthLimit = int.MaxValue;
        o.MultipartBodyLengthLimit = int.MaxValue;
        o.MultipartBoundaryLengthLimit = int.MaxValue;
        o.MultipartHeadersCountLimit = int.MaxValue;
        o.MultipartHeadersLengthLimit = int.MaxValue;
        o.BufferBodyLengthLimit = int.MaxValue;
        o.BufferBody = true;
        o.ValueCountLimit = int.MaxValue;
    });

    builder.Services
        .AddSingleton<IDateTimeProvider, DateTimeProvider>()
        .AddScoped<IDbContextWrapper, DbContextWrapper>()
        .AddScoped<IGuidProvider, GuidProvider>()
        .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
        .AddTransient<IEmailService, EmailService>()
        .AddTransient<IEmailDeliveryStatusService, EmailDeliveryStatusService>()
        .AddTransient<IEmployerInterestService, EmployerInterestService>()
        .AddTransient<IProviderDataService, ProviderDataService>()
        .AddTransient<ITownDataService, TownDataService>()
        .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
        .AddTransient<IEmployerInterestRepository, EmployerInterestRepository>()
        .AddTransient<IIndustryRepository, IndustryRepository>()
        .AddTransient<INotificationRepository, NotificationRepository>()
        .AddTransient<IProviderRepository, ProviderRepository>()
        .AddTransient<IQualificationRepository, QualificationRepository>()
        .AddTransient<IRouteRepository, RouteRepository>()
        .AddTransient<ISearchFilterRepository, SearchFilterRepository>()
        .AddTransient<ITownRepository, TownRepository>();

    builder.Services
        .AddTransient<ICacheService, MemoryCacheService>();

    builder.Services.AddNotifyService(
        siteConfiguration.EmailSettings.GovNotifyApiKey);

    builder.Services.AddQuartzServices(
        siteConfiguration.CourseDirectoryImportSchedule,
        siteConfiguration.TownDataImportSchedule,
        siteConfiguration.EmployerInterestSettings?.CleanupJobSchedule,
        siteConfiguration.ProviderSettings?.NotificationEmailJobSchedule);

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

    builder.Services.AddHealthChecks()
        .AddSqlServer(siteConfiguration.SqlConnectionString, 
            tags: new[] { "database" });

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
            "/swagger/v3/swagger.json",
            "T Levels Find a Provider.Api v3"));
    }

    //Add before CORS policy so this will be allowed through
    app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

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
    var message = $"Startup failed.\n{ex.Message}.\n{ex.StackTrace}";
    Console.WriteLine(message);

    var appInsightsInstrumentationKey = Environment
        .GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
    var appInsightsConnectionString = Environment
        .GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(appInsightsInstrumentationKey))
    {
        var client = new TelemetryClient(new TelemetryConfiguration
        {
            ConnectionString = appInsightsConnectionString
        });
        client.TrackTrace(message, SeverityLevel.Critical);
    }
    //TODO: Get rid of this branch once the above is working
    else if (!string.IsNullOrEmpty(appInsightsInstrumentationKey))
    {
        var client = new TelemetryClient(TelemetryConfiguration.CreateDefault())
        {
            InstrumentationKey = appInsightsInstrumentationKey
        };

        client.TrackTrace(message, SeverityLevel.Critical);
    }
}