using Sfa.Tl.Find.Provider.Infrastructure.Configuration;

// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class SettingsBuilder
{
    private const string AppId = "2b1c1371f07a4add85a54b1812b2b0de";
    private const string ApiKey = "be1a8d303ea04e10810eed67f5cf174a";

    private const string BlobStorageConnectionString = "UseDevelopmentStorage=true;";
    private const string SqlConnectionString = "Data Source=Test;Initial Catalog=Test;Integrated Security=True;";
    private const string RedisCacheConnectionString = "test.redis.cache.windows.net:6380,password='Test1',ssl=True,abortConnect=False";

    private const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";
    private const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
    public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);

    private const string GovNotifyApiKey = "1fff7b5b-bf64-4af9-9857-1797d0d525a3";
    private const string GovNotifyDeliveryStatusToken = "2d5230c5-6dd5-4b9d-8006-564f9568d386";

    private const string GoogleMapsApiKey = "a6d49b031a124597b7dc9bc793738040";
    private const string GoogleMapsBaseUri = "https://maps.googleapis.com/maps/api/";

    private const string PostcodeRetrieverUri = "https://test.api.postcodes.io/";
    private const bool MergeAdditionalProviderData = true;

    private const string EmployerSupportCleanupJobSchedule = "0 0 3 ? * MON-FRI";
    private const int EmployerInterestExpiryNotificationDays = 7;
    private const int EmployerInterestRetentionDays = 10;
    private const int EmployerInterestSearchRadius = 30;
    private const string EmployerSupportSiteUri = "https://test.employerssupportgov.uk/";
    private const string RegisterInterestUri = "https://test.employerssupportgov.uk/registerinterest";
    private const string ExtendEmployerUri = "https://test.employerssupportgov.uk/extendinterest/";
    private const string UnsubscribeEmployerUri = "https://test.employerssupportgov.uk/unsubscribeinterest/";

    private const string SupportEmailAddress = "support@test-email.gov.uk";

    private const string SignInAdministrators = "tester@test.com";
    private const string SignInApiUri = "https://test.api.oidc.com";
    private const string SignInApiSecret = "apisecret";
    private const string SignInAudience = "signin.oidc.com";
    private const string SignInAuthority = "https://test.signin.oidc.com";
    private const string SignInClientId = "testclient";
    private const string SignInIssuer = "testissuer";
    private const string SignInClientSecret = "secretsecret";
    private const string SignInMetadataAddress = "https://test.signin.oidc.com/metadata";
    private const int SignInTimeout = 30;

    public ApiSettings BuildApiSettings(
        string appId = AppId,
        string apiKey = ApiKey) => new()
        {
            AppId = appId,
            ApiKey = apiKey
        };

    public ConnectionStringSettings BuildConnectionStringSettings(
        string blobStorageConnectionString = BlobStorageConnectionString,
        string sqlConnectionString = SqlConnectionString,
        string redisCacheConnectionString = RedisCacheConnectionString) => new()
        {
            BlobStorageConnectionString = blobStorageConnectionString,
            SqlConnectionString = sqlConnectionString,
            RedisCacheConnectionString = redisCacheConnectionString
        };

    public CourseDirectoryApiSettings BuildCourseDirectoryApiSettings(
        string findCourseApiBaseUri = FindCourseApiBaseAbsoluteUri,
        string findCourseApiKey = FindCourseApiKey) => new()
        {
            BaseUri = findCourseApiBaseUri,
            ApiKey = findCourseApiKey
        };

    public DfeSignInSettings BuildDfeSignInSettings(
        string metadataAddress = SignInMetadataAddress,
        string administrators = SignInAdministrators,
        string audience = SignInAudience,
        string authority = SignInAuthority,
        string clientId = SignInClientId,
        string clientSecret = SignInClientSecret,
        string apiUri = SignInApiUri,
        string apiSecret = SignInApiSecret,
        string issuer = SignInIssuer,
        int timeout = SignInTimeout) => new()
        {
            Administrators = administrators,
            Audience = audience,
            Authority = authority,
            ClientId = clientId,
            ClientSecret = clientSecret,
            ApiUri = apiUri,
            ApiSecret = apiSecret,
            MetadataAddress = metadataAddress,
            Issuer = issuer,
            Timeout = timeout
        };

    public EmailSettings BuildEmailSettings(
        string govNotifyApiKey = GovNotifyApiKey,
        string deliveryStatusToken = GovNotifyDeliveryStatusToken,
        string supportEmailAddress = SupportEmailAddress) => new()
        {
            GovNotifyApiKey = govNotifyApiKey,
            DeliveryStatusToken = deliveryStatusToken,
            SupportEmailAddress = supportEmailAddress
        };

    public EmployerInterestSettings BuildEmployerInterestSettings(
        string cleanupJobSchedule = EmployerSupportCleanupJobSchedule,
        string employerSupportSiteUri = EmployerSupportSiteUri,
        string extendEmployerUri = ExtendEmployerUri,
        string registerInterestUri = RegisterInterestUri,
        string unsubscribeEmployerUri = UnsubscribeEmployerUri,
        int expiryNotificationDays = EmployerInterestExpiryNotificationDays,
        int retentionDays = EmployerInterestRetentionDays,
        int searchRadius = EmployerInterestSearchRadius) => new()
        {
            CleanupJobSchedule = cleanupJobSchedule,
            EmployerSupportSiteUri = employerSupportSiteUri,
            ExpiryNotificationDays = expiryNotificationDays,
            ExtendEmployerUri = extendEmployerUri,
            RetentionDays = retentionDays,
            SearchRadius = searchRadius,
            RegisterInterestUri = registerInterestUri,
            UnsubscribeEmployerUri = unsubscribeEmployerUri
        };

    public GoogleMapsApiSettings BuildGoogleMapsApiSettings(
        string apiKey = GoogleMapsApiKey,
        string baseUri = GoogleMapsBaseUri) => new()
        {
            ApiKey = apiKey,
            BaseUri = baseUri
        };

    public PostcodeApiSettings BuildPostcodeApiSettings(
        string postcodeRetrieverUri = PostcodeRetrieverUri) => new()
        {
            BaseUri = postcodeRetrieverUri
        };

    public SearchSettings BuildSearchSettings(
        bool mergeAdditionalProviderData = MergeAdditionalProviderData) => new()
        {
            MergeAdditionalProviderData = mergeAdditionalProviderData
        };

    public SiteConfiguration BuildConfigurationOptions(
        ApiSettings apiSettings = null,
        CourseDirectoryApiSettings courseDirectoryApiSettings = null,
        DfeSignInSettings dfeSignInSettings = null,
        EmailSettings emailSettings = null,
        EmployerInterestSettings employerInterestSettings = null,
        GoogleMapsApiSettings googleMapsApiSettings = null,
        PostcodeApiSettings postcodeApiSettings = null,
        SearchSettings searchSettings = null,
        string blobStorageConnectionString = BlobStorageConnectionString,
        string sqlConnectionString = SqlConnectionString,
        string redisCacheConnectionString = RedisCacheConnectionString,
        string courseDirectoryImportSchedule = "0 0 9 * * MON-FRI",
        string townDataImportSchedule = "0 0 10 * * MON-FRI") => new()
        {
            ApiSettings = apiSettings ?? BuildApiSettings(),
            CourseDirectoryApiSettings = courseDirectoryApiSettings ?? BuildCourseDirectoryApiSettings(),
            DfeSignInSettings = dfeSignInSettings ?? BuildDfeSignInSettings(),
            EmailSettings = emailSettings ?? BuildEmailSettings(),
            EmployerInterestSettings = employerInterestSettings ?? BuildEmployerInterestSettings(),
            GoogleMapsApiSettings = googleMapsApiSettings ?? BuildGoogleMapsApiSettings(),
            PostcodeApiSettings = postcodeApiSettings ?? BuildPostcodeApiSettings(),
            SearchSettings = searchSettings ?? BuildSearchSettings(),
            BlobStorageConnectionString = blobStorageConnectionString,
            SqlConnectionString = sqlConnectionString,
            RedisCacheConnectionString = redisCacheConnectionString,
            CourseDirectoryImportSchedule = courseDirectoryImportSchedule,
            TownDataImportSchedule = townDataImportSchedule
        };
}