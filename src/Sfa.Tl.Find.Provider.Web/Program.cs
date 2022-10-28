using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Web.ParameterTransformers;
using Sfa.Tl.Find.Provider.Web.Security;

var builder = WebApplication.CreateBuilder(args);

var siteConfiguration = builder.Configuration.LoadConfigurationOptions();
builder.Services.AddConfigurationOptions(siteConfiguration);

builder.Services
    .AddApplicationInsightsTelemetry();

//var siteConfiguration = builder.Configuration.LoadConfigurationOptions();
//builder.Services.AddConfigurationOptions(siteConfiguration);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//builder.Services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
//builder.Services.AddSingleton<IAuthorizationHandler, EmployerInterestViewerAuthorizationHandler>();
//builder.Services.AddAuthorizationServicePolicies();

if (bool.TryParse(builder.Configuration[Constants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
{
    builder.Services.AddProviderStubAuthentication();
}
else
{
    builder.Services.AddProviderAuthentication(siteConfiguration.DfeSignInSettings, builder.Environment);
}

builder.Services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, EmployerInterestViewerAuthorizationHandler>();
builder.Services.AddAuthorizationServicePolicies();


//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
//    options.CheckConsentNeeded = _ => true;
//    options.Secure = CookieSecurePolicy.Always;
//    options.MinimumSameSitePolicy = SameSiteMode.None;
//});

builder.Services.AddResponseCaching();

builder.Services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer()));
    options.Conventions.AddPageRoute("/EmployerInterest/Index", "/test-employer-list");
    options.Conventions.AddPageRoute("/Help/Cookies", "/cookies");
    options.Conventions.AddPageRoute("/Help/Privacy", "/privacy");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Start");
    options.Conventions.AllowAnonymousToPage("/Accessibility");
    options.Conventions.AllowAnonymousToPage("/Help/Cookies");
    options.Conventions.AllowAnonymousToPage("/Help/Privacy");
    options.Conventions.AllowAnonymousToPage("/TermsAndConditions");
});
builder.Services.AddControllers();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

builder.Services
    .AddCorsPolicy(Constants.CorsPolicyName,
        siteConfiguration.AllowedCorsOrigins,
        HttpMethod.Get.Method,
        HttpMethod.Post.Method);

builder.Services
    .AddPolicyRegistry()
    .AddDapperRetryPolicy()
    .AddGovNotifyRetryPolicy();

builder.Services.AddHttpClients();

builder.Services
    .AddScoped<IDateTimeService, DateTimeService>()
    .AddScoped<IDbContextWrapper, DbContextWrapper>()
    .AddScoped<IGuidService, GuidService>()
    .AddTransient<IDfeSignInTokenService, DfeSignInTokenService>()
    .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
    .AddTransient<IEmailService, EmailService>()
    .AddTransient<IEmailDeliveryStatusService, EmailDeliveryStatusService>()
    .AddTransient<IEmployerInterestService, EmployerInterestService>()
    .AddTransient<IProviderDataService, ProviderDataService>()
    .AddTransient<ITownDataService, TownDataService>()
    .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
    .AddTransient<IEmployerInterestRepository, EmployerInterestRepository>()
    .AddTransient<IIndustryRepository, IndustryRepository>()
    .AddTransient<IProviderRepository, ProviderRepository>()
    .AddTransient<IQualificationRepository, QualificationRepository>()
    .AddTransient<IRouteRepository, RouteRepository>()
    .AddTransient<ITownRepository, TownRepository>();

builder.Services.AddNotifyService(
    siteConfiguration.EmailSettings?.GovNotifyApiKey);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseXContentTypeOptions()
    .UseReferrerPolicy(opts => opts.NoReferrer());

app.UseWhen(ctx =>
        ctx.Request.Path.Value.DoesNotMatch(Constants.CssPathPattern, Constants.JsPathPattern, Constants.FontsPathPattern),
        appBuilder =>
            appBuilder
                .UseCsp(options => options
                    .FrameAncestors(s => s.None())
                    .ObjectSources(s => s.None())
                    .ScriptSources(s => s
                        .CustomSources("https:",
                            "https://www.google-analytics.com/analytics.js",
                            "https://www.googletagmanager.com/",
                            "https://tagmanager.google.com/")
                        .UnsafeInline()
                    ))
                .Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Expect-CT", "max-age=0, enforce");
                    context.Response.Headers.Add("Permissions-Policy", SecurityPolicies.PermissionsList);
                    await next.Invoke();
                })
    );

if (!string.IsNullOrWhiteSpace(siteConfiguration.AllowedCorsOrigins))
{
    app.UseCors(Constants.CorsPolicyName);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseWhen(ctx =>
        ctx.Request.Path.Value.DoesNotMatch(Constants.CssPathPattern, Constants.JsPathPattern),
    appBuilder =>
        appBuilder.UseXXssProtection(opts => opts.EnabledWithBlockMode()));

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallback("{*path}",
    () => Results.Redirect("/Error/404"));

app.MapRazorPages();
app.MapControllers();

app.UseResponseCaching();

app.Run();

// ReSharper disable once UnusedMember.Global - Required so tests can see this class
public partial class Program { }
