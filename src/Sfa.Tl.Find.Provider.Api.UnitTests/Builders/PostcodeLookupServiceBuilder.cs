using System;
using System.Collections.Generic;
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

        public PostcodeLookupService Build(
            IHttpClientFactory httpClientFactory = null,
            ILogger<PostcodeLookupService> logger = null)
        {
            httpClientFactory ??= Substitute.For<IHttpClientFactory>();
            logger ??= Substitute.For<ILogger<PostcodeLookupService>>();

            return new PostcodeLookupService(httpClientFactory, logger);
        }

        //public PostcodeLookupService Build(
        //    string targetUriFragment,
        //    PostcodeLookupJsonBuilder dataBuilder,
        //    ILogger<PostcodeLookupService> logger = null)
        //{
        //    var targetUri = new Uri(baseUri, targetUriFragment);

        //    var httpClientFactory = Substitute.For<IHttpClientFactory>();
        //    httpClientFactory
        //        .CreateClient()
        //        .Returns(new TestHttpClientFactory()
        //            .CreateHttpClient(targetUri,
        //                dataBuilder.Build()));

        //    return Build(httpClientFactory, logger);
        //}

        public PostcodeLookupService Build(
            IDictionary<string, HttpResponseMessage> responseMessages,
            ILogger<PostcodeLookupService> logger = null)
        {
            var responsesWithUri = new Dictionary<Uri, HttpResponseMessage>();
            foreach (var item in responseMessages)
            {
                responsesWithUri.Add(
                    new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                    item.Value);
            }

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient()
                .Returns(new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri));

            return Build(httpClientFactory, logger);

        }

        public PostcodeLookupService Build(
            IDictionary<string, string> responseMessages,
            ILogger<PostcodeLookupService> logger = null)
        {
            var responsesWithUri = new Dictionary<Uri, string>();
            foreach (var item in responseMessages)
            {
                responsesWithUri.Add(
                    new Uri(PostcodeRetrieverApiBaseUri, item.Key),
                    item.Value);
            }

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient()
                .Returns(new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(PostcodeRetrieverApiBaseUri, responsesWithUri));

            return Build(httpClientFactory, logger);
        }
    }
}
