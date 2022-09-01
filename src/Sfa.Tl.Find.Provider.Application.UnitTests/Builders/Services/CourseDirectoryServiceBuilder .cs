using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class CourseDirectoryServiceBuilder
{
    private const string CourseDirectoryApiBaseAbsoluteUri = "https://https://test.com/findacourse/api/";
    private static readonly Uri CourseDirectoryApiBaseUri = new(CourseDirectoryApiBaseAbsoluteUri);

    public ICourseDirectoryService Build(
        HttpClient httpClient = null, 
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IMemoryCache cache = null,
        ILogger<CourseDirectoryService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        providerRepository ??= Substitute.For<IProviderRepository>();
        qualificationRepository ??= Substitute.For<IQualificationRepository>();
        cache ??= Substitute.For<IMemoryCache>();
        logger ??= Substitute.For<ILogger<CourseDirectoryService>>();

        return new CourseDirectoryService(
            httpClient, 
            providerRepository, 
            qualificationRepository, 
            cache, 
            logger);
    }
        
    public ICourseDirectoryService Build(
        IDictionary<string, string> responseMessages,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IMemoryCache cache = null,
        ILogger<CourseDirectoryService> logger = null)
    {
        var responsesWithUri = responseMessages?
            .ToDictionary(
                item => new Uri(CourseDirectoryApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(CourseDirectoryApiBaseUri, responsesWithUri);

        return Build(
            httpClient, 
            providerRepository, 
            qualificationRepository, 
            cache, 
            logger);
    }
}