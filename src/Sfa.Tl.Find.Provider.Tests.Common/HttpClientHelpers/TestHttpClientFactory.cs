using System.Net;

namespace Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

public class TestHttpClientFactory
{
    public HttpClient CreateHttpClientWithBaseUri(
        Uri? baseUri, 
        IDictionary<Uri, string> responseMessages, 
        string responseContentType = "application/json", 
        HttpStatusCode responseCode = HttpStatusCode.OK)
    {
        var fakeMessageHandler = new FakeHttpMessageHandler();
        foreach (var (key, value) in responseMessages)
        {
            var httpResponseMessage = FakeResponseFactory.CreateFakeResponse(value, responseContentType, responseCode);
            fakeMessageHandler.AddFakeResponse(key, httpResponseMessage);
        }

        var httpClient = new HttpClient(fakeMessageHandler)
        {
            BaseAddress = baseUri
        };

        return httpClient;
    }

    public HttpClient CreateHttpClientWithBaseUri(
        Uri baseUri, 
        IDictionary<Uri, HttpResponseMessage> httpResponseMessages)
    {
        var fakeMessageHandler = new FakeHttpMessageHandler();
        foreach (var (key, value) in httpResponseMessages)
        {
            fakeMessageHandler.AddFakeResponse(key, value);
        }

        var httpClient = new HttpClient(fakeMessageHandler)
        {
            BaseAddress = baseUri
        };

        return httpClient;
    }
}