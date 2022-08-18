using Sfa.Tl.Find.Provider.Application.Models.Configuration;

// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public class SettingsBuilder
{
    private const string AppId = "2b1c1371f07a4add85a54b1812b2b0de";
    private const string ApiKey = "be1a8d303ea04e10810eed67f5cf174a";
    private const string ConnectionString = "Data Source=Test;Initial Catalog=Test;Integrated Security=True;";
    private const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";
    private const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
    public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);
    private const string GovNotifyApiKey = "1fff7b5b-bf64-4af9-9857-1797d0d525a3";
    private const string PostcodeRetrieverUri = "https://test.api.postcodes.io/";
    private const bool MergeAdditionalProviderData = true;
    private const string SupportEmailAddress = "support@test-email.gov.uk";

    public ApiSettings BuildApiSettings(
        string appId = AppId,
        string apiKey = ApiKey) => new()
        {
            AppId = appId,
            ApiKey = apiKey
        };

    public ConnectionStringSettings BuildConnectionStringSettings(
        string connectionString = ConnectionString) => new()
        {
            SqlConnectionString = connectionString
        };

    public CourseDirectoryApiSettings BuildCourseDirectoryApiSettings(
        string findCourseApiBaseUri = FindCourseApiBaseAbsoluteUri,
        string findCourseApiKey = FindCourseApiKey) => new()
        {
            BaseUri = findCourseApiBaseUri,
            ApiKey = findCourseApiKey
        };

    public EmailSettings BuildEmailSettings(
    string govNotifyApiKey = GovNotifyApiKey,
    string supportEmailAddress = SupportEmailAddress) => new()
    {
        GovNotifyApiKey = govNotifyApiKey,
        SupportEmailAddress = supportEmailAddress
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
        EmailSettings emailSettings = null,
        PostcodeApiSettings postcodeApiSettings = null,
        SearchSettings searchSettings = null,
        string sqlConnectionString = "TestConnection",
        string courseDirectoryImportSchedule = "0 0 9 * * MON-FRI",
        string townDataImportSchedule = "0 0 10 * * MON-FRI") => new()
        {
            ApiSettings = apiSettings ?? BuildApiSettings(),
            CourseDirectoryApiSettings = courseDirectoryApiSettings ?? BuildCourseDirectoryApiSettings(),
            EmailSettings = emailSettings ?? BuildEmailSettings(),
            PostcodeApiSettings = postcodeApiSettings ?? BuildPostcodeApiSettings(),
            SearchSettings = searchSettings ?? BuildSearchSettings(),
            SqlConnectionString = sqlConnectionString,
            CourseDirectoryImportSchedule = courseDirectoryImportSchedule,
            TownDataImportSchedule = townDataImportSchedule
        };
}