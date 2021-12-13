using System;
using FluentAssertions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Caching
{
    public class CacheKeysTests
    {
        [Theory(DisplayName = nameof(Models.CacheKeys.PostcodeKey) + " Data Tests")]
        [InlineData("cv12wt", "POSTCODE__CV12WT")]
        [InlineData("CV1 2WT", "POSTCODE__CV12WT")]
        public void Postcode_Key_Returns_Expected_Value(string postcode, string expectedKey)
        {
            var key = Models.CacheKeys.PostcodeKey(postcode);
            key.Should().Be(expectedKey);
        }

        [Fact]
        public void PostcodeKey_Throws_Exception_For_Null_Postcode()
        {
            Action act = () => Models.CacheKeys.PostcodeKey(null);

            act.Should().Throw<ArgumentNullException>();

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("postcode");
        }

        [Fact]
        public void PostcodeKey_Throws_Exception_For_Empty_Postcode()
        {
            Action act = () => Models.CacheKeys.PostcodeKey("");

            act.Should().Throw<ArgumentException>();

            act.Should().Throw<ArgumentException>()
                .WithMessage("A non-empty postcode is required*")
                .WithParameterName("postcode");
        }
    }
}
