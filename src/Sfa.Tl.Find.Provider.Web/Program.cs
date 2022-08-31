using Microsoft.AspNetCore.Authorization;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Web.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationInsightsTelemetry();

var siteConfiguration = builder.Configuration.LoadConfigurationOptions();
builder.Services.AddConfigurationOptions(builder.Configuration, siteConfiguration);

builder.Services.AddSingleton<IAuthorizationHandler, ProviderAuthorizationHandler>();
builder.Services.AddAuthorizationServicePolicies();

if(bool.TryParse(builder.Configuration["StubProviderAuth"], out var isStubProviderAuth) && isStubProviderAuth)
{
    builder.Services.AddProviderStubAuthentication();
}
else
{
    builder.Services.AddProviderAuthentication(siteConfiguration.DfeSignInSettings);
}

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddResponseCaching();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Index");
    //options.Conventions.AuthorizeFolder("/Private");
});

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
    });
}
builder.Services
    .AddPolicyRegistry()
    .AddDapperRetryPolicy();

builder.Services.AddHttpClients();

builder.Services
    .AddScoped<IDateTimeService, DateTimeService>()
    .AddScoped<IDbContextWrapper, DbContextWrapper>()
    .AddTransient<IDynamicParametersWrapper, DynamicParametersWrapper>()
    .AddTransient<IEmailService, EmailService>()
    .AddTransient<IProviderDataService, ProviderDataService>()
    .AddTransient<ITownDataService, TownDataService>()
    .AddTransient<IEmailTemplateRepository, EmailTemplateRepository>()
    .AddTransient<IProviderRepository, ProviderRepository>()
    .AddTransient<IQualificationRepository, QualificationRepository>()
    .AddTransient<IRouteRepository, RouteRepository>()
    .AddTransient<ITownRepository, TownRepository>();

builder.Services.AddNotifyService(
    siteConfiguration.EmailSettings.GovNotifyApiKey);

var webRootPath = builder.Environment.WebRootPath;
var contentRootPath = builder.Environment.ContentRootPath;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseReferrerPolicy(opts => opts.NoReferrer())
    //.UseXXssProtection(opts => opts.EnabledWithBlockMode());
    ;

app.UseWhen(
    ctx => IsNotImgFile(ctx.Request.Path),
    appBuilder =>
        appBuilder.UseXContentTypeOptions());

app.UseWhen(
        ctx => IsNotCssOrImgOrFontFile(ctx.Request.Path),
        appBuilder =>
            appBuilder
                //.UseXContentTypeOptions()
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


//app.UseWhen(
//    ctx => IsCssOrJsFile(ctx.Request.Path),
//    appBuilder => appBuilder.UseXContentTypeOptions());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseWhen(
    ctx => IsNotJsOrCssFile(ctx.Request.Path),
    appBuilder =>
        appBuilder.UseXXssProtection(opts => opts.EnabledWithBlockMode()));

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallback("{*path}",
    () => Results.Redirect("/Error/404"));

app.MapRazorPages();

app.UseResponseCaching();

app.Run();

//var _fp = new PhysicalFileProvider()//
bool IsNotJsFile(string path)
{
    if (string.IsNullOrEmpty(path)) return false;
    var starts = !string.IsNullOrEmpty(webRootPath) && path.StartsWith(webRootPath);
    var starts2 = !string.IsNullOrEmpty(webRootPath) && path.StartsWith(contentRootPath);
    return !path.EndsWith(".js");
}

bool IsNotImgFile(string path)
{
    var starts = path.StartsWith(webRootPath);
    var starts2 = path.StartsWith(contentRootPath);
    return !path.Contains("/assets/images/");
}

bool IsNotJsOrCssFile(string path)
{
    var starts = path.StartsWith(webRootPath);
    var starts2 = path.StartsWith(contentRootPath);
    return !path.EndsWith(".css") && !path.EndsWith(".js");
}

bool IsNotCssOrImgOrFontFile(string path)
{
    var starts = path.StartsWith(webRootPath);
    var starts2 = path.StartsWith(contentRootPath);
    return !path.Contains(".css") &&
           !path.Contains("/assets/fonts/") &&
           !path.Contains("/assets/images/");
}

public partial class Program { } //Required so tests can see this class
