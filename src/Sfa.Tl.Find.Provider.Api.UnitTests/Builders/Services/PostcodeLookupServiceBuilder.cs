using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;
using Sfa.Tl.Find.Provider.Application.Services;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Services;

public class PostcodeLookupServiceBuilder
{
    private const string PostcodeRetrieverApiBaseAbsoluteUri = "https://test.api.postcodes.io/";
    private static readonly Uri PostcodeRetrieverApiBaseUri = new(PostcodeRetrieverApiBaseAbsoluteUri);

    // ReSharper disable once MemberCanBePrivate.Global
    public PostcodeLookupService Build(
        HttpClient httpClient = null)
    {
        httpClient ??= Substitute.For<HttpClient>();

        return new PostcodeLookupService(httpClient);
    }

    public PostcodeLookupService Build(
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

    public PostcodeLookupService Build(
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