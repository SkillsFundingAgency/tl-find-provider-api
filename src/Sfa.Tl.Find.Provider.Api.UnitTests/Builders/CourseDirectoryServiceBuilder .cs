using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class CourseDirectoryServiceBuilder
    {
        private const string CourseDirectoryApiBaseAbsoluteUri = "https://https://test.com/findacourse/api/";
        private static readonly Uri CourseDirectoryApiBaseUri = new(CourseDirectoryApiBaseAbsoluteUri);

        // ReSharper disable once MemberCanBePrivate.Global
        public CourseDirectoryService Build(
            HttpClient httpClient = null, 
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            ILogger<CourseDirectoryService> logger = null)
        {
            httpClient ??= Substitute.For<HttpClient>();
            providerRepository ??= Substitute.For<IProviderRepository>();
            qualificationRepository ??= Substitute.For<IQualificationRepository>();
            logger ??= Substitute.For<ILogger<CourseDirectoryService>>();

            return new CourseDirectoryService(httpClient, providerRepository, qualificationRepository, logger);
        }

        public CourseDirectoryService Build(
            IDictionary<string, HttpResponseMessage> responseMessages,
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            ILogger<CourseDirectoryService> logger = null)
        {
            var responsesWithUri = responseMessages
                .ToDictionary(
                    item => new Uri(CourseDirectoryApiBaseUri, item.Key),
                    item => item.Value);

            var httpClient = new TestHttpClientFactory()
                .CreateHttpClientWithBaseUri(CourseDirectoryApiBaseUri, responsesWithUri);

            return Build(httpClient, providerRepository, qualificationRepository, logger);
        }

        public CourseDirectoryService Build(
            IDictionary<string, string> responseMessages,
            IProviderRepository providerRepository = null,
            IQualificationRepository qualificationRepository = null,
            ILogger<CourseDirectoryService> logger = null)
        {
            var responsesWithUri = responseMessages
                .ToDictionary(
                    item => new Uri(CourseDirectoryApiBaseUri, item.Key),
                    item => item.Value);

            var httpClient = new TestHttpClientFactory()
                    .CreateHttpClientWithBaseUri(CourseDirectoryApiBaseUri, responsesWithUri);

            return Build(httpClient, providerRepository, qualificationRepository, logger);
        }
    }
}
