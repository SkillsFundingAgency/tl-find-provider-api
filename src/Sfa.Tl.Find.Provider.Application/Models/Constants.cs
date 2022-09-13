namespace Sfa.Tl.Find.Provider.Application.Models;

public static class Constants
{
    public const string CorsPolicyName = "CorsPolicy";

    public const string DapperRetryPolicyName = "dapper-transient-error-retry";

    public const int DefaultPageSize = 5;

    public const int DefaultApiMajorVersion = 1;
    public const int DefaultApiMinorVersion = 0;

    public const int DefaultAbsoluteExpirationInMinutes = 60;
    public const int DefaultSlidingExpirationInMinutes = 10;

    public const double DefaultLatitude = 51.477928;
    public const double DefaultLongitude = 0;

    public const string EnvironmentNameConfigKey = "EnvironmentName";
    public const string ConfigurationStorageConnectionStringConfigKey = "ConfigurationStorageConnectionString";
    public const string VersionConfigKey = "Version";
    public const string ServiceNameConfigKey = "ServiceName";

    public const string StartupTasksJobKeyName = "Perform Startup Tasks";
    public const string CourseDirectoryImportJobKeyName = "Import Course Data";

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
}