using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Attributes;

public class HmacAuthorizationAttributeTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(HmacAuthorizationAttribute)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Implementation_Type_Is_Expected_Type()
    {
        var attribute = new HmacAuthorizationAttribute();
        attribute.ImplementationType.Should().Be(typeof(HmacAuthorizationFilter));
    }

    [Fact]
    public void Implementation_Create_Instance_Returns_Expected_Object()
    {
        var apiSettingOptions = Options.Create(
            new SettingsBuilder()
                .BuildApiSettings());

        var memoryCache = Substitute.For<IMemoryCache>();
        var logger = Substitute.For<ILogger<HmacAuthorizationFilter>>();

        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => apiSettingOptions)
            .AddScoped(_ => memoryCache)
            .AddScoped(_ => logger)
            .BuildServiceProvider();

        var attribute = new HmacAuthorizationAttribute();
        attribute.ImplementationType.Should().Be(typeof(HmacAuthorizationFilter));

        var filter = attribute.CreateInstance(serviceProvider);
        filter.Should().NotBeNull();
        filter.Should().BeOfType(typeof(HmacAuthorizationFilter));
    }
}