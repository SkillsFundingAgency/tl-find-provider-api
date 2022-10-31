using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class PostcodeLookupServiceBuilder
{
    private const string PostcodeRetrieverApiBaseAbsoluteUri = "https://test.api.postcodes.io/";
    private static readonly Uri PostcodeRetrieverApiBaseUri = new(PostcodeRetrieverApiBaseAbsoluteUri);

    public IPostcodeLookupService Build(
        HttpClient httpClient = null,
        IDateTimeService dateTimeService = null,
        ICacheService cacheService = null,
        ILogger<PostcodeLookupService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        cacheService ??= Substitute.For<ICacheService>();
        dateTimeService ??= Substitute.For<IDateTimeService>();
        logger ??= Substitute.For<ILogger<PostcodeLookupService>>();

        return new PostcodeLookupService(
            httpClient,
            dateTimeService,
            cacheService,
            logger);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, HttpResponseMessage> responseMessages,
        IDateTimeService dateTimeService = null,
        ICacheService cacheService = null,
        ILogger<PostcodeLookupService> logger = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(
            httpClient,
            dateTimeService,
            cacheService,
            logger);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, string> responseMessages,
        IDateTimeService dateTimeService = null,
        ICacheService cacheService = null,
        ILogger<PostcodeLookupService> logger = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(
            httpClient,
            dateTimeService,
            cacheService,
            logger);
    }
}