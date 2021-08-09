namespace Sfa.Tl.Find.Provider.Api.Models.Configuration
{
    public class SiteConfiguration
    {
        public string AllowedOrigins { get; init; }
        public string SqlConnectionString { get; init; }
        public CourseDirectoryApiSettings CourseDirectoryApiSettings { get; init; }
        public PostcodeApiSettings PostcodeApiSettings { get; init; }
    }
}
