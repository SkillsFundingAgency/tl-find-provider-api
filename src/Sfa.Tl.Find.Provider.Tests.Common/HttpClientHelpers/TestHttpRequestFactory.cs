using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

public static class TestHttpRequestFactory
{
    public static HttpRequest Create(
        HttpMethod method,
        Uri uri,
        string payload)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var httpContext = new DefaultHttpContext()
        {
            Request =
            {
                Method = method.ToString(),
                Host = uri.Port > 0 && !uri.IsDefaultPort
                    ? new HostString(uri.Host, uri.Port)
                    : new HostString(uri.Host),
                Body = stream,
                ContentLength = stream.Length
            }
        };

        return httpContext.Request;
    }

    public static HttpRequest Create(
        HttpMethod method,
        Uri uri,
        object body = null)
    {
        var payload = body != null
            ? JsonSerializer.Serialize(body)
            : "";

        return Create(method, uri, payload);
    }
}

