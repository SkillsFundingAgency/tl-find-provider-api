using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.HealthChecks;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Providers;
using Sfa.Tl.Find.Provider.Infrastructure.Services;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Web.Filters;
using Sfa.Tl.Find.Provider.Web.Middleware;
using Sfa.Tl.Find.Provider.Web.Security;
using ConfigurationConstants = Sfa.Tl.Find.Provider.Infrastructure.Configuration.Constants;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

var builder = WebApplication.CreateBuilder(args);

var siteConfiguration = builder.Configuration.LoadConfigurationOptions();

builder.Services
    .AddApplicationInsightsTelemetry();

builder.Services.AddConfigurationOptions(siteConfiguration);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

if (bool.TryParse(builder.Configuration[ConfigurationConstants.SkipProviderAuthenticationConfigKey], out var isStubProviderAuth) && isStubProviderAuth)
{
    builder.Services.AddProviderStubAuthentication();
}
else
{
    builder.Services.AddProviderAuthentication(siteConfiguration.DfeSignInSettings!, builder.Environment);
    builder.Services.AddWebDataProtection(siteConfiguration, builder.Environment);
}

builder.Services.AddSingleton<IAuthorizationHandler, AdministratorAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ProviderOrAdministratorAuthorizationHandler>();
builder.Services.AddAuthorizationPolicies();

builder.Services.AddResponseCaching();

builder.Services.Configure<RouteOptions>(option =>
{
    option.LowercaseUrls = true;
    option.LowercaseQueryStrings = true;
});

builder.Services.AddRazorPages(options =>
{
    //options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer()));
    options.Conventions.AddPageRoute("/Employer/EmployerList", "/employer-list");
    options.Conventions.AddPageRoute("/Employer/EmployerDetails", "/employer-details");
    options.Conventions.AddPageRoute("/Employer/RemoveEmployer", "/remove-employer");
    options.Conventions.AddPageRoute("/Help/AccessibilityStatement", "/accessibility-statement");
    options.Conventions.AddPageRoute("/Help/Cookies", "/cookies");
    options.Conventions.AddPageRoute("/Help/Privacy", "/privacy");
    options.Conventions.AddPageRoute("/Help/TermsAndConditions", "/terms-and-conditions");
    options.Conventions.AddPageRoute("/Provider/SearchFilters", "/filters");
    options.Conventions.AddPageRoute("/Provider/SearchFilterDetails", "/filters-edit");
    options.Conventions.AddPageRoute("/Provider/Notifications", "/notifications");
    options.Conventions.AddPageRoute("/Provider/AddNotification", "/notifications-add-email");
    options.Conventions.AddPageRoute("/Provider/AddAdditionalNotification", "/notifications-add-email-campus");
    options.Conventions.AddPageRoute("/Provider/EditNotification", "/notifications-edit");
    options.Conventions.AddPageRoute("/Provider/RemoveNotification", "/notifications-remove");
    options.Conventions.AddPageRoute("/Provider/AddNotificationLocation", "/notifications-edit-campus-add");
    options.Conventions.AddPageRoute("/Provider/EditNotificationLocation", "/notifications-edit-campus");

    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Start");
    options.Conventions.AllowAnonymousToPage("/AccessibilityStatement");
    options.Conventions.AllowAnonymousToPage("/Help/Cookies");
    options.Conventions.AllowAnonymousToPage("/Help/Privacy");
    options.Conventions.AllowAnonymousToPage("/TermsAndConditions");
})
    .AddMvcOptions(options =>
    {
        options.Filters.Add<UserSessionActivityPageFilter>();
    })
    .AddSessionStateTempDataProvider();

builder.Services.AddControllers();

builder.Services
    .AddTransient<IHttpContextAccessor, HttpContextAccessor>()
    .AddSession(options =>
    {
        options.Cookie.Name = ".cookies.session";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.IdleTimeout = TimeSpan.FromMinutes(siteConfiguration.DfeSignInSettings?.Timeout ?? 20);
        options.Cookie.IsEssential = true;
    })
    .AddTransient<ISessionService>(x =>
        new SessionService(
            x.GetService<IHttpContextAccessor>()!,
            builder.Environment.EnvironmentName));

builder.Services.AddHealthChecks();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

builder.Services
    .AddPolicyRegistry()
    .AddDapperRetryPolicy()
    .AddGovNotifyRetryPolicy();

builder.Services.AddHttpClients();

builder.Services
    .AddSingleton<IDateTimeProvider, DateTimeProvider>()
    .AddScoped<IDbContextWrapper, DbContextWrapper>()
    .AddScoped<IGuidProvider, GuidProvider>()
    .AddTransient<IDfeSignInTokenService, DfeSignInTokenService>()
    .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
    .AddTransient<IEmailService, EmailService>()
    .AddTransient<IEmployerInterestService, EmployerInterestService>()
    .AddTransient<INotificationService, NotificationService>()
    .AddTransient<IProviderDataService, ProviderDataService>()
    .AddTransient<ISearchFilterService, SearchFilterService>()
    .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
    .AddTransient<IEmployerInterestRepository, EmployerInterestRepository>()
    .AddTransient<IIndustryRepository, IndustryRepository>()
    .AddTransient<INotificationRepository, NotificationRepository>()
    .AddTransient<IProviderRepository, ProviderRepository>()
    .AddTransient<IQualificationRepository, QualificationRepository>()
    .AddTransient<IRouteRepository, RouteRepository>()
    .AddTransient<ISearchFilterRepository, SearchFilterRepository>()
    .AddTransient<ITownRepository, TownRepository>();

builder.Services.AddCachingServices(siteConfiguration.RedisCacheConnectionString);

builder.Services.AddNotifyService(
    siteConfiguration.EmailSettings?.GovNotifyApiKey);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.SetStrictTransportSecurityHeader();
}

app.UseXContentTypeOptions()
    .UseReferrerPolicy(opts => opts.NoReferrer());

app.UseXfo(xfo => xfo.Deny());

app.UseWhen(ctx =>
        ctx.Request.Path.Value.DoesNotMatch(Constants.CssPathPattern, Constants.JsPathPattern, Constants.FontsPathPattern),
        appBuilder =>
            appBuilder
                .SetConfigSecurityPolicyHeader()
                .Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Expect-CT", "max-age=0, enforce");
                    context.Response.Headers.Add("Permissions-Policy", SecurityPolicies.PermissionsList);
                    await next.Invoke();
                })
    );

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseWhen(ctx =>
        ctx.Request.Path.Value.DoesNotMatch(Constants.CssPathPattern, Constants.JsPathPattern),
    appBuilder =>
        appBuilder.UseXXssProtection(opts => opts.EnabledWithBlockMode()));

app.UseRouting();

app.UseSession();
app.UseMiddleware<SessionMiddleware>();

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
