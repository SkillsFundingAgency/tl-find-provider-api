using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Extensions;

public class FormattingExtensionsTests
{
    [Theory(DisplayName = nameof(FormattingExtensions.FormatDistance) + " Data Tests")]
    [InlineData(0D, "0 miles")]
    [InlineData(1.2D, "1 mile")]
    [InlineData(5.4D, "5 miles")]
    [InlineData(10D, "10 miles")]
    public void FormatDistance_For_Double_Data_Tests(double distance, string expectedResult)
    {
        var result = distance.FormatDistance();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(FormattingExtensions.FormatDistance) + "Nullable Data Tests")]
    [InlineData(null, "")]
    [InlineData(0.0, "0 miles")]
    [InlineData(1.2D, "1 mile")]
    [InlineData(10D, "10 miles")]
    public void FormatDistance_For_Nullable_Double_Data_Tests(double? distance, string expectedResult)
    {
        var result = distance.FormatDistance();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(FormattingExtensions.FormatPostcodeForUri) + " Data Tests")]
    [InlineData("CV1 2WT", "CV1%202WT")]
    [InlineData("cv1 2wt", "CV1%202WT")]
    [InlineData(" CV1 2WT ", "CV1%202WT")]
    public void FormatPostcodeForUri_Data_Tests(string input, string expectedResult)
    {
        var result = input.FormatPostcodeForUri();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(FormattingExtensions.FormatTownName) + " Data Tests")]
    [InlineData("Oxford", "Oxfordshire", "Oxfordshire", "Oxford, Oxfordshire")]
    [InlineData("Bristol", null, "Gloucestershire", "Bristol, Gloucestershire")]
    [InlineData("Coventry", "West Midlands", "West Midlands", "Coventry, West Midlands")]
    [InlineData("Coventry", "", "", "Coventry")]
    [InlineData("Some Town (Somewhere)", "Some County", null, "Some Town (Somewhere), Some County")]
    public void FormatTownName_Data_Tests(string name, string county, string localAuthority, string expectedResult)
    {
        var town = new Town
        {
            Name = name,
            County = county,
            LocalAuthority = localAuthority
        };

        var result = town.FormatTownName();
        result.Should().Be(expectedResult);
    }

    [Theory(DisplayName = nameof(FormattingExtensions.GetContactPreferenceDisplayName) + " Data Tests")]
    [InlineData(null, "")]
    [InlineData(ContactPreference.Email, "Email")]
    [InlineData(ContactPreference.Telephone, "Telephone")]
    [InlineData(ContactPreference.NoPreference, "No preference")]
    [InlineData((ContactPreference)4, "Unknown")]
    [InlineData((ContactPreference)5, "No idea", "No idea")]
    public void GetContactPreferenceDisplayName_Data_Tests(ContactPreference? contactPreference, string expectedResult, string defaultValue = null)
    {
        var result = defaultValue is null 
            ? contactPreference.GetContactPreferenceDisplayName()
            : contactPreference.GetContactPreferenceDisplayName(defaultValue);

        result.Should().Be(expectedResult);
    }
}