using System.Net;
using System.Net.Http.Headers;

namespace Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

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
}