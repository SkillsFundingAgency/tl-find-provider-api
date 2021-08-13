using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class PostcodeLookupServiceBuilder
    {
        private const string PostcodeRetrieverApiBaseAbsoluteUri = "https://test.api.postcodes.io/";
        private static readonly Uri PostcodeRetrieverApiBaseUri = new(PostcodeRetrieverApiBaseAbsoluteUri);

        // ReSharper disable once MemberCanBePrivate.Global
        public PostcodeLookupService Build(
            HttpClient httpClient = null,
            ILogger<PostcodeLookupService> logger = null)
        {
            httpClient ??= Substitute.For<HttpClient>();
            logger ??= Substitute.For<ILogger<PostcodeLookupService>>();

            return new PostcodeLookupService(httpClient, logger);
        }

        public PostcodeLookupService Build(
            IDictionary<string, HttpResponseMessage> responseMessages,
            ILogger<PostcodeLookupService> logger = null)
        {
            var responsesWithUri = responseMessages
                .ToDictionary(
                    item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                    item => item.Value);

            var httpClient = new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

            return Build(httpClient, logger);
        }

        public PostcodeLookupService Build(
            IDictionary<string, string> responseMessages,
            ILogger<PostcodeLookupService> logger = null)
        {
            var responsesWithUri = responseMessages
                .ToDictionary(
                    item => new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                    item => item.Value);

            var httpClient = new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri);

            return Build(httpClient, logger);
        }
    }
}
