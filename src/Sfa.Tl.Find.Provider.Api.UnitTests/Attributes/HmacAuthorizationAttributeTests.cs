using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Attributes
{
    public class HmacAuthorizationAttributeTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
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

        //TODO: Add a test for CreateInstance - needs a ServiceProvider passed in
    }
}
