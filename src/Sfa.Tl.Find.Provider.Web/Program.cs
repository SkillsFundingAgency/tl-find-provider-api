using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Extensions;
using Sfa.Tl.Find.Provider.Web.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationInsightsTelemetry();

var siteConfiguration = builder.Configuration.LoadConfigurationOptions();
//TODO: Add config
//builder.Services
//    .Configure<LinkSettings>(x =>
//    {
//        x.ConfigureLinkSettings(siteConfiguration.LinkSettings);
//    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = _ => true;
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddResponseCaching();

builder.Services.AddRazorPages();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

var _webRootPath = builder.Environment.WebRootPath;
var _contentRootPath = builder.Environment.ContentRootPath;

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
    //|| ctx..Response.Re.Result is ViewResult
    app =>
        app.UseXContentTypeOptions());

app.UseWhen(
        ctx => IsNotCssOrImgOrFontFile(ctx.Request.Path)
        //|| ctx..Response.Re.Result is ViewResult
        ,
   app =>
    app//.UseXContentTypeOptions()
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
//    app => app.UseXContentTypeOptions());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseWhen(
    ctx => IsNotJsOrCssFile(ctx.Request.Path),
    app =>
        app.UseXXssProtection(opts => opts.EnabledWithBlockMode()));

app.UseRouting();

app.UseAuthorization();

app.MapFallback("{*path}",
    () => Results.Redirect("/Error/404"));

app.MapRazorPages();

app.UseResponseCaching();

app.Run();

//var _fp = new PhysicalFileProvider()//
bool IsNotJsFile(string path)
{
    var starts = path.StartsWith(_webRootPath);
    var starts2 = path.StartsWith(_contentRootPath);
    return !path.EndsWith(".js");
}

bool IsNotImgFile(string path)
{
    var starts = path.StartsWith(_webRootPath);
    var starts2 = path.StartsWith(_contentRootPath);
    return !path.Contains("/assets/images/");
}

bool IsNotJsOrCssFile(string path)
{
    var starts = path.StartsWith(_webRootPath);
    var starts2 = path.StartsWith(_contentRootPath);
    return !path.EndsWith(".css") && !path.EndsWith(".js");
}

bool IsNotCssOrImgOrFontFile(string path)
{
    var starts = path.StartsWith(_webRootPath);
    var starts2 = path.StartsWith(_contentRootPath);
    return !path.Contains(".css") &&
           !path.Contains("/assets/fonts/") &&
           !path.Contains("/assets/images/");
}

public partial class Program { } //Required so tests can see this class

