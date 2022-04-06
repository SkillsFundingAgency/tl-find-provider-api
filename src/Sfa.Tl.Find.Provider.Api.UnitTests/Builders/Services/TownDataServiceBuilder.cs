using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Services;

public class TownDataServiceBuilder
{
    public TownDataService Build(
        HttpClient httpClient = null,
        ITownRepository townRepository = null,
        ILogger<TownDataService> logger = null)
    {
        httpClient ??= Substitute.For<HttpClient>();
        townRepository ??= Substitute.For<ITownRepository>();
        logger ??= Substitute.For<ILogger<TownDataService>>();

        return new TownDataService(
            httpClient,
            townRepository,
            logger);
    }

    public TownDataService Build(
        IDictionary<string, string> responseMessages,
        ITownRepository townRepository = null,
        ILogger<TownDataService> logger = null)
    {
        var responsesWithUri = responseMessages
            .ToDictionary(
                item => new Uri(item.Key),
                item => item.Value);

        var httpClient = new TestHttpClientFactory()
            .CreateHttpClientWithBaseUri(null, responsesWithUri);

        return Build(
            httpClient, 
            townRepository, 
            logger);
    }
}