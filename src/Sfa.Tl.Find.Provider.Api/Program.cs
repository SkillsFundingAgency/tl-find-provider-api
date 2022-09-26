using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;

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
        .AddCorsPolicy(Constants.CorsPolicyName, siteConfiguration.AllowedCorsOrigins)
        .AddPolicyRegistry()
        .AddDapperRetryPolicy();

    builder.Services.AddHttpClients();

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = int.MaxValue;
    });

    builder.Services
        .AddScoped<IDateTimeService, DateTimeService>()
        .AddScoped<IDbContextWrapper, DbContextWrapper>()
        .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
        .AddTransient<IEmailService, EmailService>()
        .AddTransient<IEmailDeliveryStatusService, EmailDeliveryStatusService>()
        .AddTransient<IEmployerInterestService, EmployerInterestService>()
        .AddTransient<IProviderDataService, ProviderDataService>()
        .AddTransient<ITownDataService, TownDataService>()
        .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
        .AddTransient<IEmployerInterestRepository, EmployerInterestRepository>()
        .AddTransient<IProviderRepository, ProviderRepository>()
        .AddTransient<IQualificationRepository, QualificationRepository>()
        .AddTransient<IRouteRepository, RouteRepository>()
        .AddTransient<ITownRepository, TownRepository>();

    builder.Services.AddNotifyService(
        siteConfiguration.EmailSettings.GovNotifyApiKey);

    builder.Services.AddQuartzServices(
        siteConfiguration.CourseDirectoryImportSchedule,
        siteConfiguration.TownDataImportSchedule,
        siteConfiguration.EmployerInterestSettings?.CleanupJobSchedule);

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
            "/swagger/v3/swagger.json",
            "T Levels Find a Provider.Api v3"));
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