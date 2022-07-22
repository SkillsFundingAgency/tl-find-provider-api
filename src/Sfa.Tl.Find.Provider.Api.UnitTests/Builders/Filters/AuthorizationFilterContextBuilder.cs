using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;

public class AuthorizationFilterContextBuilder
{
    public AuthorizationFilterContext Build(
        string requestUri,
        IHeaderDictionary requestHeaders = null,
        string method = "GET",
        Stream stream = null)
    {
        if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

        var uri = new Uri(requestUri);

        var bodyStream = stream ?? new MemoryStream();
        bodyStream.Position = 0;

        var httpRequest = new HttpRequestFeature
        {
            Method = method,
            Scheme = uri.Scheme,
            Path = uri.AbsolutePath,
            QueryString = uri.Query,
            Body = bodyStream
        };

        if (requestHeaders != null && requestHeaders.Any())
        {
            httpRequest.Headers = requestHeaders;
        }

        var features = new FeatureCollection();
        features.Set<IHttpRequestFeature>(httpRequest);

        var httpContext = new DefaultHttpContext(features)
        {
            Request =
            {
                Host = uri.Port > 0 && !uri.IsDefaultPort
                    ? new HostString(uri.Host, uri.Port)
                    : new HostString(uri.Host),
                Body = bodyStream,
                ContentLength = bodyStream.Length
            }
        };

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor(),
        };

        return new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());
    }
}