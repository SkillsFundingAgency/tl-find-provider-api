using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class TownDataServiceBuilder
{
    public ITownDataService Build(
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

    public ITownDataService Build(
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