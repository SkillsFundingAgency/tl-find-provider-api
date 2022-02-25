using System;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sfa.Tl.Find.Provider.Api.Connected_Services.Sfa.Tl.Find.Provider.Api.UkRlp.Api.Client;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;

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

builder.Services.AddSwagger("v1",
    "T Levels Find a Provider Api",
    "v1",
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

builder.Services
    .AddCorsPolicy(Constants.CorsPolicyName, siteConfiguration.AllowedCorsOrigins)
    .AddPolicyRegistry()
    .AddDapperRetryPolicy();

builder.Services.AddHttpClients();

builder.Services.AddTransient<IProviderQueryPortTypeClient>(_ =>
{
    var client = new ProviderQueryPortTypeClient();

    var timeoutTimeSpan = new TimeSpan(0, 5, 0);
    client.Endpoint.Binding.SendTimeout = timeoutTimeSpan;
    client.Endpoint.Binding.ReceiveTimeout = timeoutTimeSpan;

    return client;
});

builder.Services
    .AddScoped<IDbContextWrapper, DbContextWrapper>()
    .AddScoped<IDateTimeService, DateTimeService>()
    .AddTransient<IProviderDataService, ProviderDataService>()
    .AddTransient<IProviderReferenceDataService, ProviderReferenceDataService>()
    .AddTransient<IProviderRepository, ProviderRepository>()
    .AddTransient<IProviderReferenceRepository, ProviderReferenceRepository>()
    .AddTransient<IQualificationRepository, QualificationRepository>()
    .AddTransient<IRouteRepository, RouteRepository>();

builder.Services.AddQuartzServices(siteConfiguration.CourseDirectoryImportSchedule);

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
        "/swagger/v1/swagger.json",
        "T Levels Find a Provider.Api v1"));
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
