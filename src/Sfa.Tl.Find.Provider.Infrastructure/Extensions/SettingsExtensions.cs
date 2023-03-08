using Sfa.Tl.Find.Provider.Infrastructure.Configuration;

namespace Sfa.Tl.Find.Provider.Infrastructure.Extensions;
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

        settings.BlobStorageConnectionString = configuration.BlobStorageConnectionString;
        settings.SqlConnectionString = configuration.SqlConnectionString;
        settings.RedisCacheConnectionString = configuration.RedisCacheConnectionString;
    }

    public static void ConfigureCourseDirectoryApiSettings(this CourseDirectoryApiSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.BaseUri = configuration.CourseDirectoryApiSettings?.BaseUri;
        settings.ApiKey = configuration.CourseDirectoryApiSettings?.ApiKey;
    }

    public static void ConfigureDfeSignInSettings(this DfeSignInSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.Administrators = configuration.DfeSignInSettings?.Administrators;
        settings.ApiUri = configuration.DfeSignInSettings?.ApiUri;
        settings.ApiSecret = configuration.DfeSignInSettings?.ApiSecret;
        settings.Audience = configuration.DfeSignInSettings?.Audience;
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
        settings.ExtendEmployerUri = configuration.EmployerInterestSettings?.ExtendEmployerUri;
        settings.ExpiryNotificationDays = configuration.EmployerInterestSettings?.ExpiryNotificationDays ?? 0;
        settings.MaximumExtensions = configuration.EmployerInterestSettings?.MaximumExtensions ?? 0;
        settings.RetentionDays = configuration.EmployerInterestSettings?.RetentionDays ?? 0;
        settings.UnsubscribeEmployerUri = configuration.EmployerInterestSettings?.UnsubscribeEmployerUri;
        settings.RegisterInterestUri = configuration.EmployerInterestSettings?.RegisterInterestUri;
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

    public static void ConfigureProviderSettings(this ProviderSettings settings, SiteConfiguration configuration)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        settings.ConnectSiteUri = configuration.ProviderSettings?.ConnectSiteUri;
        settings.DefaultSearchRadius = configuration.ProviderSettings?.DefaultSearchRadius ?? 0;
        settings.DefaultNotificationSearchRadius = configuration.ProviderSettings?.DefaultNotificationSearchRadius ?? 0;
        settings.NotificationEmailImmediateSchedule = configuration.ProviderSettings?.NotificationEmailImmediateSchedule;
        settings.NotificationEmailDailySchedule = configuration.ProviderSettings?.NotificationEmailDailySchedule;
        settings.NotificationEmailWeeklySchedule = configuration.ProviderSettings?.NotificationEmailWeeklySchedule;
        settings.SupportSiteAccessConnectHelpUri = configuration.ProviderSettings?.SupportSiteAccessConnectHelpUri;
    }
}
