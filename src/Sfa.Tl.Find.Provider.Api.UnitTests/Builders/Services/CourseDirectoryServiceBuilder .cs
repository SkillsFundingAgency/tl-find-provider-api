using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Services;

public class CourseDirectoryServiceBuilder
{
    private const string CourseDirectoryApiBaseAbsoluteUri = "https://https://test.com/findacourse/api/";
    private static readonly Uri CourseDirectoryApiBaseUri = new(CourseDirectoryApiBaseAbsoluteUri);

    // ReSharper disable once MemberCanBePrivate.Global
    public CourseDirectoryService Build(
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
        
    public CourseDirectoryService Build(
        IDictionary<string, string> responseMessages,
        IProviderRepository providerRepository = null,
        IQualificationRepository qualificationRepository = null,
        IMemoryCache cache = null,
        ILogger<CourseDirectoryService> logger = null)
    {
        var responsesWithUri = responseMessages
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