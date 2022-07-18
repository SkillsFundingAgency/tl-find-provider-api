using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class HttpClientBuilderExtensionsTests
{
    [Fact]
    public void AddRetryPolicyHandler_Should_AddService()
    {
        var services = new ServiceCollection();
        var builder = Substitute.For<IHttpClientBuilder>();
        builder.Services.Returns(services);

        var builderAfter = builder.AddRetryPolicyHandler<HttpClientBuilderExtensionsTests>();

        builderAfter.Should().NotBeNull();
        builderAfter.Services.Should().NotBeNullOrEmpty();
    }
}