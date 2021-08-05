using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.HttpClientHelpers
{
    public static class FakeResponseFactory
    {
        public static HttpResponseMessage CreateFakeResponse(string response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = new HttpResponseMessage(responseCode)
            {
                Content = new StringContent(response)
            };

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

            return httpResponseMessage;
        }

        public static HttpResponseMessage CreateFakeResponse(Stream response, string responseContentType = "application/json", HttpStatusCode responseCode = HttpStatusCode.OK)
        {
            var httpResponseMessage = new HttpResponseMessage(responseCode)
            {
                Content = new StreamContent(response)
            };
            
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

            return httpResponseMessage;
        }
    }
}
