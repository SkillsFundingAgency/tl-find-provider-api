
namespace Sfa.Tl.Find.Provider.Api.Models.Configuration;

public class SiteConfiguration
{
    public string AllowedCorsOrigins { get; init; }
    public ApiSettings ApiSettings { get; init; }
    public CourseDirectoryApiSettings CourseDirectoryApiSettings { get; init; }
    public string CourseDirectoryImportSchedule { get; init; }
    public string EmployerRegistrationEmailTemplateId { get; init; }
    public string EmployerRegistrationInboxAddress { get; init; }
    public string GovNotifyApiKey { get; init; }
    public PostcodeApiSettings PostcodeApiSettings { get; init; }
    public SearchSettings SearchSettings { get; init; }
    public string SqlConnectionString { get; init; }
    public string TownDataImportSchedule { get; init; }
}