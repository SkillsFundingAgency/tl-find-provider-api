using System.Text;
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
        Stream stream = null,
        string method = "GET")
    {
        if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));
            
        var uri = new Uri(requestUri);

        //var stream = payload is not null 
        //    ? new MemoryStream(Encoding.UTF8.GetBytes(payload))
        //    : new MemoryStream();

        var bodyStream = stream ?? new MemoryStream();
        bodyStream.Position = 0;

        var httpRequest = new HttpRequestFeature
        {
            Method = method,
            Scheme = uri.Scheme,
            Path = uri.AbsolutePath,
            QueryString = uri.Query,
            Body = bodyStream
            //ContentLength = stream.Length;
        };

        if (requestHeaders != null && requestHeaders.Any())
        {
            httpRequest.Headers = requestHeaders;
        }

        var features = new FeatureCollection();
        features.Set<IHttpRequestFeature>(httpRequest);
        features.Set<IHttpResponseFeature>(new HttpResponseFeature());
        features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(Stream.Null));

        var httpContext = new DefaultHttpContext(features)
        //var httpContext = new DefaultHttpContext()
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

        //https://zditect.com/code/csharp/deeply-explore-the-correct-way-for-aspnet-core-to-read-requestbody.html
        //httpContext.Request.EnableBuffering();
        //var bodyPipe = new RequestBodyPipeFeature(stream);
        //var bodyPipe = new RequestBodyPipeFeature(httpContext);
        //var bodyPipe = new StreamResponseBodyFeature(stream);
        //features.Set<IHttpResponseBodyFeature>(bodyPipe);
        //features.Set<IRequestBodyPipeFeature>(bodyPipe);
        //var bodyPipeStream = new StreamResponseBodyFeature(stream);

        //features.Re
        //features.Set<IRequestBodyPipeFeature>(bodyPipe);
        //httpContext.Initialize(features);

        var actionContext = new ActionContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor(),
        };


        //var r = httpContext.Request.BodyReader;


        return new AuthorizationFilterContext(
            actionContext,
            new List<IFilterMetadata>());
    }
}