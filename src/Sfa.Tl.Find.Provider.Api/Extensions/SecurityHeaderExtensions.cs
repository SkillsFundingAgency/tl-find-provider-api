namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class SecurityHeaderExtensions
{
    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
    {
        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .RemoveServerHeader()
            .AddCrossOriginOpenerPolicy(builder =>
            {
                builder.SameOrigin();
            })
            .AddCrossOriginEmbedderPolicy(builder =>
            {
                builder.RequireCorp();
            })
            .AddCrossOriginResourcePolicy(builder =>
            {
                builder.SameOrigin();
            })
            .RemoveServerHeader()
            .AddPermissionsPolicy(builder =>
            {
                builder.AddAccelerometer().None();
                builder.AddAutoplay().None();
                builder.AddCamera().None();
                builder.AddEncryptedMedia().None();
                builder.AddFullscreen().All();
                builder.AddGeolocation().None();
                builder.AddGyroscope().None();
                builder.AddMagnetometer().None();
                builder.AddMicrophone().None();
                builder.AddMidi().None();
                builder.AddPayment().None();
                builder.AddPictureInPicture().None();
                builder.AddSyncXHR().None();
                builder.AddUsb().None();
            })
            .AddCspHstsDefinitions(isDev);

        return policy;
    }

    private static HeaderPolicyCollection AddCspHstsDefinitions(this HeaderPolicyCollection policy, bool isDev)
    {
        if (!isDev)
        {
            policy.AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().None();
                builder.AddFormAction().None();
                builder.AddFontSrc().None();
                builder.AddStyleSrc().Self();
                builder.AddScriptSrc().Self().UnsafeInline();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();
                builder.AddCustomDirective("require-trusted-types-for", "'script'");
            });

            policy
                .AddStrictTransportSecurityMaxAgeIncludeSubDomains(
                    maxAgeInSeconds: 60 * 60 * 24 * 365); //one year in seconds
        }
        else
        {
            // allow swagger UI for dev
            policy.AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From("data:");
                builder.AddFormAction().Self();
                builder.AddFontSrc().Self();
                builder.AddStyleSrc().Self().UnsafeInline();
                builder.AddScriptSrc().Self().UnsafeInline();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();
            });
        }

        return policy;
    }
}