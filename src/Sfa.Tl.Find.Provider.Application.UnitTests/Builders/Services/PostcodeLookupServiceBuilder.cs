using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class PostcodeLookupServiceBuilder
{
    private const string PostcodeRetrieverApiBaseAbsoluteUri = "https://test.api.postcodes.io/";
    private static readonly Uri PostcodeRetrieverApiBaseUri = new(PostcodeRetrieverApiBaseAbsoluteUri);

    public IPostcodeLookupService Build(
        HttpClient httpClient = null)
    {
        httpClient ??= Substitute.For<HttpClient>();

        return new PostcodeLookupService(httpClient);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, HttpResponseMessage> responseMessages)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(httpClient);
    }

    public IPostcodeLookupService Build(
        IDictionary<string, string> responseMessages)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

        return Build(httpClient);
    }
}