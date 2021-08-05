using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory(DisplayName = nameof(StringExtensions.FormatPostcodeForUri) + " Data Tests")]
        [InlineData("CV1 2WT", "CV1%202WT")]
        [InlineData("cv1 2wt", "CV1%202WT")]
        [InlineData(" CV1 2WT ", "CV1%202WT")]
        public void StringExtensionsDataTests(string input, string expectedResult)
        {
            var result = input.FormatPostcodeForUri();
            result.Should().Be(expectedResult);
        }
    }
}
