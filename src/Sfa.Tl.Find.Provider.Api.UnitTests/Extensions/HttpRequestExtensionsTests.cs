using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class HttpRequestExtensionsTests
{
    [Fact]
    public async Task GetRawBodyAsync_Returns_Expected_Result()
    {
        //const string payload = "{ \"test\": 1 }";
        //var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        //TODO: use object - qualification? Then check serialized string
        const string payload = "{ \"test\": 1 }";
        var httpRequest = CreateMockRequest(payload);

        var result = await httpRequest.GetRawBodyAsync();

        result.Should().Be(payload);
    }

    [Fact]
    public async Task GetRawBodyBytesAsync_Returns_Expected_Result()
    {
        //TODO: use object - qualification? Then check serialized string
        const string payload = "{ \"test\": 1 }";

        //var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        var bytes = Encoding.UTF8.GetBytes(payload);

        var httpRequest = CreateMockRequest(payload);

        var result = await httpRequest.GetRawBodyBytesAsync();

        result.Should().BeEquivalentTo(bytes);
    }

    private HttpRequest CreateMockRequest(string payload
        , object body = null)
    {
        //var byteArray = Encoding.ASCII.GetBytes(json);

        //_memoryStream = new MemoryStream(byteArray);
        //_memoryStream.Flush();
        //_memoryStream.Position = 0;

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var uri = new Uri("https://test/test");
        var httpContext = new DefaultHttpContext()//features)
        {
            Request =
            {
                Method = HttpMethod.Post.ToString(),
                Host = uri.Port > 0 && !uri.IsDefaultPort
                    ? new HostString(uri.Host, uri.Port)
                    : new HostString(uri.Host),
                Body = stream,
                ContentLength = stream.Length
            }
        };

        return httpContext.Request;
        //var msg = new HttpRequestMessage();

        //var json = JsonConvert.SerializeObject(body);
        //var byteArray = Encoding.ASCII.GetBytes(json);

        //_memoryStream = new MemoryStream(byteArray);
        //_memoryStream.Flush();
        //_memoryStream.Position = 0;

        //var mockRequest = new Subs<HttpRequest>();
        //mockRequest.Setup(x => x.Body).Returns(_memoryStream);

        //return mockRequest;
    }
}