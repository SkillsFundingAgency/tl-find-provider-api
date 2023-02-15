namespace Sfa.Tl.Find.Provider.Application.Models;

public static class Constants
{
    public const string CorsPolicyName = "CorsPolicy";

    public const string DapperRetryPolicyName = "dapper-transient-error-retry";
    public const string GovNotifyRetryPolicyName = "gov-uk-notify-transient-error-retry";

    public const int DefaultPageSize = 5;

    public const int DefaultProviderNotificationFilterRadius = 5;
    public const int DefaultProviderSearchFilterRadius = 20;

    public const int DefaultApiMajorVersion = 1;
    public const int DefaultApiMinorVersion = 0;

    public const double DefaultLatitude = 51.477928;
    public const double DefaultLongitude = 0;
    
    public const int ProviderNameMaxLength = 400;
    public const int LocationNameMaxLength = 400;
    public const int QualificationNameMaxLength = 400;
    public const int PostcodeMaxLength = 10;
    public const int AddressLineMaxLength = 100;
    public const int TownMaxLength = 100;
    public const int CountyMaxLength = 50;
    public const int EmailMaxLength = 320;
    public const int TelephoneMaxLength = 150;
    public const int WebsiteMaxLength = 150;

    public const int TownSearchDefaultMaxResults = 50;

    public const string CssPathPattern = @"^\/css\/.*\.css$";
    public const string FontsPathPattern = @"^\/assets\/fonts\/.*\.woff\d?$";
    public const string JsPathPattern = @"^\/js\/.*\.js$";
}