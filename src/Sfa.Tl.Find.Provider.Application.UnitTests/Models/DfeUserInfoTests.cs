using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models.Authentication;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Models;

public class DfeUserInfoTests
{
    [Theory(DisplayName = nameof(StringExtensions.FormatPostcodeForUri) + " Data Tests")]
    [InlineData("Bob", null, "Bob")]
    [InlineData("Bob", "Jones", "Bob Jones")]
    [InlineData(null, "Sting", "Sting")]
    public void String_FormatPostcodeForUri_Data_Tests(string firstName, string surname, string expectedResult)
    {
        var userInfo = new DfeUserInfo
        {
            FirstName = firstName,
            Surname = surname
        };

        userInfo.DisplayName.Should().Be(expectedResult);
    }
}
