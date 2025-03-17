namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder SetStrictTransportSecurityHeader(this IApplicationBuilder app)
    {
        const int Days = 365;

        return app.UseHsts(options =>
        {
            options.MaxAge(Days);
            options.Preload();
            options.IncludeSubdomains();
        });
    }

    public static IApplicationBuilder SetConfigSecurityPolicyHeader(this IApplicationBuilder app)
    {
        string[] strictDynamicCustomSources = { "https:", "https://www.google-analytics.com/analytics.js", "https://www.googletagmanager.com/", "https://tagmanager.google.com/" };

        return app.UseCsp(options => options
            .FrameAncestors(s => s.None())
            .ScriptSources(conf => conf
                .CustomSources(strictDynamicCustomSources)
                .UnsafeInline())
            .ObjectSources(s => s.None()));
    }
}