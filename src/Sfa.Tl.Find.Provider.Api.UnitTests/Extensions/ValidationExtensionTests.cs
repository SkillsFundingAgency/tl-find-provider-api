using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class ValidationExtensionTests
{
    [Fact]
    public void Validate_Null_SearchTerm_Returns_Expected_Results()
    {
        const string searchTerm = null;
        searchTerm!.TryValidate(out var errorMessage).Should().BeFalse();
        errorMessage.Should().Be("The postcode field is required.");
    }

    [Theory(DisplayName = nameof(ValidationExtensions.TryValidate) + " Search Term Data Tests")]
    [InlineData("CV1 2WT", true, null)]
    [InlineData(null, false, "The postcode field is required.")]
    [InlineData("", false, "The postcode field is required.")]
    public void Validate_SearchTerm_Data_Tests(string searchTerm, bool expectedValidationResult, string expectedErrorMessage)
    {
        searchTerm!.TryValidate(out var errorMessage).Should().Be(expectedValidationResult);
        errorMessage.Should().Be(expectedErrorMessage);
    }

    [Theory(DisplayName = nameof(ValidationExtensions.TryValidate) + " Lat/Long Data Tests")]
    [InlineData(51.0, -2.0, true, null)]
    [InlineData(null, null, false, "Both latitude and longitude required if postcode is not provided.")]
    public void Validate_Latitude_Longitude_Data_Tests(double? latitude, double? longitude, bool expectedValidationResult, string expectedErrorMessage)
    {
        (latitude, longitude)!.TryValidate(out var errorMessage).Should().Be(expectedValidationResult);
        errorMessage.Should().Be(expectedErrorMessage);
    }

    [Theory(DisplayName = nameof(ValidationExtensions.TryValidate) + " Search Term and Lat/Long Data Tests")]
    [InlineData(null, 51.0, -2.1, true, null)]
    [InlineData("", 51.0, -1.0, true, null)]
    [InlineData("CV1 2WT", 1.0, 2.0, false, "Either postcode or lat/long required, but not both.")]
    public void Validate_Search_Term_And_Latitude_Longitude_Data_Tests(string searchTerm, double? latitude, double? longitude, bool expectedValidationResult, string expectedErrorMessage)
    {
        searchTerm.TryValidate(latitude, longitude, out var errorMessage).Should().Be(expectedValidationResult);
        errorMessage.Should().Be(expectedErrorMessage);
    }
}