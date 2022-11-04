
namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class SiteConfiguration
{
    public string? AllowedCorsOrigins { get; init; }
    public ApiSettings? ApiSettings { get; init; }
    public CourseDirectoryApiSettings? CourseDirectoryApiSettings { get; init; }
    public string? CourseDirectoryImportSchedule { get; init; }
    public DfeSignInSettings? DfeSignInSettings { get; init; }
    public EmailSettings? EmailSettings { get; init; }
    public EmployerInterestSettings? EmployerInterestSettings { get; init; }
    public GoogleMapsApiSettings? GoogleMapsApiSettings { get; init; }
    public PostcodeApiSettings? PostcodeApiSettings { get; init; }
    public SearchSettings? SearchSettings { get; init; }
    public string? SqlConnectionString { get; init; }
    public string? RedisConnectionString { get; init; }
    public string? TownDataImportSchedule { get; init; }
}