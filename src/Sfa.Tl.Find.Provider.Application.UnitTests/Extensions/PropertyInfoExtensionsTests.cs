using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class PropertyInfoExtensionsTests
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    private class StubClassWithPropertyAttributes
    {
        [Description("Test")]
        public string StringWithAttribute { get; }

        public string StringWithoutAttribute { get; }
    }

    [Fact]
    public void Property_HasAttribute_Returns_True_When_Attribute_Is_Present()
    {
        var prop = typeof(StubClassWithPropertyAttributes)
            .GetProperties()
            .SingleOrDefault(p => p.Name == "StringWithAttribute");

        prop.HasAttribute<DescriptionAttribute>()
            .Should().BeTrue();
    }

    [Fact]
    public void Property_HasAttribute_Returns_False_When_Attribute_Is_Not_Present()
    {
        var prop = typeof(StubClassWithPropertyAttributes)
            .GetProperties()
            .SingleOrDefault(p => p.Name == "StringWithAttribute");

        prop.Should().NotBeNull();
        prop.HasAttribute<DisplayNameAttribute>()
            .Should().BeFalse();
    }

    [Fact]
    public void Property_HasAttribute_Returns_False_When_There_Are_No_Attributes()
    {
        var prop = typeof(StubClassWithPropertyAttributes)
            .GetProperties()
            .SingleOrDefault(p => p.Name == "StringWithoutAttribute");

        prop.Should().NotBeNull();
        prop.HasAttribute<DisplayNameAttribute>()
            .Should().BeFalse();
    }
}