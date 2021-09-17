using System.Text.Json;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions
{
    public class JsonExtensionsTests
    {
        private readonly JsonDocument _jsonDocument = JsonDocument.Parse(
            "{ " +
                "\"anElement\": {" +
                    "\"myInt32\": 123," +
                    "\"myInt64\": 1000000000," +
                    "\"myPositiveDouble\": 100.999," +
                    "\"myNegativeDouble\": -100.999," +
                    "\"myTrueBool\": true," +
                    "\"myFalseBool\": false," +
                    "\"myString\": \"my value\"" +
                "}" +
            "}");

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetBoolean) + " Data Tests")]
        [InlineData("myInt32", false)]
        [InlineData("myPositiveDouble", false)]
        [InlineData("myString", false)]
        [InlineData("notANumber", false)]
        [InlineData("myTrueBool", true)]
        [InlineData("myFalseBool", false)]
        public void JsonElement_SafeGetBoolean_Data_Tests(string propertyName, bool expectedResult)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetBoolean(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetInt32) + " Data Tests")]
        [InlineData("myInt32", 123)]
        [InlineData("myPositiveDouble", 0)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        [InlineData("myTrueBool", 0)]
        [InlineData("myFalseBool", 0)]
        public void JsonElement_SafeGetInt32_Data_Tests(string propertyName, int expectedResult)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt32(propertyName);

            result.Should().BeOfType(typeof(int));
            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetInt64) + " Data Tests")]
        [InlineData("myInt32", 123)]
        [InlineData("myInt64", 1000000000)]
        [InlineData("myPositiveDouble", 0)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        [InlineData("myTrueBool", 0)]
        [InlineData("myFalseBool", 0)]
        public void JsonElement_SafeGetInt64_Data_Tests(string propertyName, long expectedResult)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetInt64(propertyName);

            result.Should().BeOfType(typeof(long));
            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetDouble) + " Data Tests")]
        [InlineData("myPositiveDouble", 100.999)]
        [InlineData("myNegativeDouble", -100.999)]
        [InlineData("myInt64", 1000000000)]
        [InlineData("myString", 0)]
        [InlineData("notANumber", 0)]
        [InlineData("myTrueBool", 0)]
        [InlineData("myFalseBool", 0)]
        public void JsonElement_SafeGetDouble_Data_Tests(string propertyName, double expectedResult)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetDouble(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetString) + " Data Tests")]
        [InlineData("myString", "my value")]
        [InlineData("myInt32", null)]
        [InlineData("myInt64", null)]
        [InlineData("myDouble", null)]
        [InlineData("notAString", null)]
        [InlineData("myTrueBool", null)]
        [InlineData("myFalseBool", null)]
        public void JsonElement_SafeGetString_Data_Tests(string propertyName, string expectedResult)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetString(propertyName);

            result.Should().Be(expectedResult);
        }

        [Theory(DisplayName = nameof(JsonExtensions.SafeGetString) + " with maxLength Data Tests")]
        [InlineData("myString", "my value", 10)]
        [InlineData("myString", "my value", 8)]
        [InlineData("myString", "my val", 6)]
        [InlineData("myInt32", null, 100)]
        [InlineData("myInt64", null, 100)]
        [InlineData("myDouble", null, 100)]
        [InlineData("notAString", null, 100)]
        [InlineData("myTrueBool", null, 100)]
        [InlineData("myFalseBool", null, 100)]
        public void JsonElement_SafeGetString_With_Max_Length_Data_Tests(string propertyName, string expectedResult, int maxLength)
        {
            var prop = _jsonDocument.RootElement.GetProperty("anElement");
            var result = prop.SafeGetString(propertyName, maxLength);

            result.Should().Be(expectedResult);
        }
    }
}
