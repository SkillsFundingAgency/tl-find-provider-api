using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Providers;
using Sfa.Tl.Find.Provider.Infrastructure.Services;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Web.Filters;
using Sfa.Tl.Find.Provider.Web.Security;
using ConfigurationConstants = Sfa.Tl.Find.Provider.Infrastructure.Configuration.Constants;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;

var builder = WebApplication.CreateBuilder(args);

var siteConfiguration = builder.Configuration.LoadConfigurationOptions();

builder.Services
    .AddApplicationInsightsTelemetry();

builder.Services.AddConfigurationOptions(siteConfiguration);

builder.Services.AddMemoryCache();

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
    builder.Services.AddProviderAuthentication(siteConfiguration.DfeSignInSettings, builder.Environment);
    builder.Services.AddWebDataProtection(siteConfiguration);
}

builder.Services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ProviderUkPrnOrAdministratorAuthorizationHandler>();
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
        options.IdleTimeout = TimeSpan.FromMinutes(siteConfiguration.DfeSignInSettings.Timeout);
        options.Cookie.IsEssential = true;
    })
    .AddTransient<ISessionService>(x =>
        new SessionService(
            x.GetService<IHttpContextAccessor>()!,
            builder.Environment.EnvironmentName));

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
    .AddScoped<IDateTimeProvider, DateTimeProvider>()
    .AddScoped<IDbContextWrapper, DbContextWrapper>()
    .AddScoped<IGuidProvider, GuidProvider>()
    .AddTransient<IDfeSignInTokenService, DfeSignInTokenService>()
    .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
    .AddTransient<IEmailService, EmailService>()
    .AddTransient<IEmployerInterestService, EmployerInterestService>()
    .AddTransient<IProviderDataService, ProviderDataService>()
    .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
    .AddTransient<IEmployerInterestRepository, EmployerInterestRepository>()
    .AddTransient<IIndustryRepository, IndustryRepository>()
    .AddTransient<IProviderRepository, ProviderRepository>()
    .AddTransient<IQualificationRepository, QualificationRepository>()
    .AddTransient<IRouteRepository, RouteRepository>()
    .AddTransient<ITownRepository, TownRepository>();

builder.Services
    .AddTransient<ICacheService, MemoryCacheService>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseWhen(ctx =>
        ctx.Request.Path.Value.DoesNotMatch(Constants.CssPathPattern, Constants.JsPathPattern),
    appBuilder =>
        appBuilder.UseXXssProtection(opts => opts.EnabledWithBlockMode()));

app.UseRouting();

app.UseSession();

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
