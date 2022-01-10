using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.DelegatingHandlers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestServerFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly TestServer _server;

    public TestServerFactory()
    {
        _server = new TestServer(
            new WebHostBuilder()
                .UseStartup<TEntryPoint>());
    }

    public HttpClient CreateClient()
    {
        var apiSettings = _server.Services.GetRequiredService<IOptions<ApiSettings>>();

        var baseHandler = _server.CreateHandler();
        var hmacAuthorizationHandler = new HmacAuthorizationHeaderDelegatingHandler(apiSettings)
        {
            InnerHandler = baseHandler
        };

        var client = new HttpClient(hmacAuthorizationHandler)
        {
            BaseAddress = _server.BaseAddress
        };

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    }
}