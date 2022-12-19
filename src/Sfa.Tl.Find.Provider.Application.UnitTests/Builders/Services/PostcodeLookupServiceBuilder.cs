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
        ICacheService cacheService = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        cacheService ??= Substitute.For<ICacheService>();

        return new PostcodeLookupService(
            httpClient,
            cacheService);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, HttpResponseMessage> responseMessages,
        ICacheService cacheService = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(
            httpClient,
            cacheService);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, string> responseMessages,
        ICacheService cacheService = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(
            httpClient,
            cacheService);
    }
}