using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Application.Extensions;
public static class SettingsExtensions
{
    public static void ConfigureApiSettings(this ApiSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.AppId = configuration.ApiSettings?.AppId;
        settings.ApiKey = configuration.ApiSettings?.ApiKey;
    }

    public static void ConfigureConnectionStringSettings(this ConnectionStringSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.SqlConnectionString = configuration.SqlConnectionString;
    }

    public static void ConfigureCourseDirectoryApiSettings(this CourseDirectoryApiSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.BaseUri = configuration.CourseDirectoryApiSettings.BaseUri;
        settings.ApiKey = configuration.CourseDirectoryApiSettings.ApiKey;
    }

    public static void ConfigureDfeSignInSettings(this DfeSignInSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.ApiUri = configuration.DfeSignInSettings?.ApiUri;
        settings.ApiSecret = configuration.DfeSignInSettings?.ApiSecret;
        settings.Audience = configuration.DfeSignInSettings?.Audience;
        settings.Authority = configuration.DfeSignInSettings?.Authority;
        settings.ClientId = configuration.DfeSignInSettings?.ClientId;
        settings.ClientSecret = configuration.DfeSignInSettings?.ClientSecret;
        settings.Issuer = configuration.DfeSignInSettings?.Issuer;
        settings.MetadataAddress = configuration.DfeSignInSettings?.MetadataAddress;
        settings.Timeout = configuration.DfeSignInSettings?.Timeout ?? 0;
    }

    public static void ConfigureEmailSettings(this EmailSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.GovNotifyApiKey = configuration.EmailSettings?.GovNotifyApiKey;
        settings.DeliveryStatusToken = configuration.EmailSettings?.DeliveryStatusToken;
        settings.SupportEmailAddress = configuration.EmailSettings?.SupportEmailAddress;
    }

    public static void ConfigureEmployerInterestSettings(this EmployerInterestSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.EmployerSupportSiteUri = configuration.EmployerInterestSettings?.EmployerSupportSiteUri;
        settings.CleanupJobSchedule = configuration.EmployerInterestSettings?.CleanupJobSchedule;
        settings.RetentionDays = configuration.EmployerInterestSettings?.RetentionDays ?? 0;
        settings.UnsubscribeEmployerUri = configuration.EmployerInterestSettings?.UnsubscribeEmployerUri;
        settings.ServiceStartDate = configuration.EmployerInterestSettings?.ServiceStartDate;
        settings.SearchRadius = configuration.EmployerInterestSettings?.SearchRadius ?? 0;
    }

    public static void ConfigureGoogleMapsApiSettings(this GoogleMapsApiSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.ApiKey = configuration.GoogleMapsApiSettings?.ApiKey;
        settings.BaseUri = configuration.GoogleMapsApiSettings?.BaseUri;
    }

    public static void ConfigurePostcodeApiSettings(this PostcodeApiSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.BaseUri = configuration.PostcodeApiSettings?.BaseUri;
    }

    public static void ConfigureSearchSettings(this SearchSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.MergeAdditionalProviderData = configuration.SearchSettings?.MergeAdditionalProviderData ?? false;
    }
}
