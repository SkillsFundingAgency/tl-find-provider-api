using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests
{
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
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
