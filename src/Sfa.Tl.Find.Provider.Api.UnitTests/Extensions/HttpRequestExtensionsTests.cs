using System.Text;
using System.Text.Json;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.HttpClientHelpers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class HttpRequestExtensionsTests
{
    [Fact]
    public async Task GetRawBodyAsync_Returns_Expected_Result()
    {
        var body = new QualificationBuilder().BuildList().First();
        var json = JsonSerializer.Serialize(body);

        var httpRequest = TestHttpRequestFactory.Create(
            HttpMethod.Post,
            new Uri("https://test"),
            body);

        var result = await httpRequest.GetRawBodyAsync();

        result.Should().Be(json);
    }

    [Fact]
    public async Task GetRawBodyBytesAsync_Returns_Expected_Result()
    {
        var body = new QualificationBuilder().BuildList().First();
        var json = JsonSerializer.Serialize(body);

        var bytes = Encoding.UTF8.GetBytes(json);

        var httpRequest = TestHttpRequestFactory.Create(
            HttpMethod.Post,
            new Uri("https://test"),
            body);

        var result = await httpRequest.GetRawBodyBytesAsync();

        result.Should().BeEquivalentTo(bytes);
    }
}