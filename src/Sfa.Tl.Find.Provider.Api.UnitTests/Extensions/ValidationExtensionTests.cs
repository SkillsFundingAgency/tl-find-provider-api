using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class ValidationExtensionTests
{
    [Theory(DisplayName = nameof(ValidationExtensions.TryValidate) + " Search Term Data Tests")]
    [InlineData("CV1 2WT", true, null)]
    [InlineData(null, false, "The search term is required.")]
    [InlineData("", false, "The search term is required.")]
    public void Validate_SearchTerm_Data_Tests(string searchTerm, bool expectedValidationResult, string expectedErrorMessage)
    {
        searchTerm!.TryValidate(out var errorMessage).Should().Be(expectedValidationResult);
        errorMessage.Should().Be(expectedErrorMessage);
    }

    [Theory(DisplayName = nameof(ValidationExtensions.TryValidate) + " Search Term Data Tests")]
    // ReSharper disable StringLiteralTypo
    [InlineData("Newcastle-under-Lyme, Staffordshire", true, null)]
    [InlineData("Westward Ho!", true, null)]
    [InlineData("Wymondham(Melton), Leicestershire", true, null)]
    [InlineData("Oakthorpe & Donisthorpe", true, null)]
    [InlineData("Bede, Tyne & Wear", true, null)]
    [InlineData("Bishop's Castle, Shropshire", true, null)]
    [InlineData("Accrington/Rossendale, Lancashire", true, null)]
    // ReSharper restore StringLiteralTypo
    public void Validate_Town_Names_With_Special_Character_SearchTerm_Data_Tests(string searchTerm, bool expectedValidationResult, string expectedErrorMessage)
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
    [InlineData("CV1 2WT", 1.0, 2.0, false, "Either search term or lat/long required, but not both.")]
    public void Validate_Search_Term_And_Latitude_Longitude_Data_Tests(string searchTerm, double? latitude, double? longitude, bool expectedValidationResult, string expectedErrorMessage)
    {
        searchTerm.TryValidate(latitude, longitude, out var errorMessage).Should().Be(expectedValidationResult);
        errorMessage.Should().Be(expectedErrorMessage);
    }
}