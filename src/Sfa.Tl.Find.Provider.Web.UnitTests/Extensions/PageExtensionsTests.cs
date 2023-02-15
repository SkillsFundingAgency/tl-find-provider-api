using System.Security.Claims;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Extensions;

public class PageExtensionsTests
{
    private const string DefaultServiceName = "Connect with employers interested in T Levels";

    [Fact]
    public void GetServiceName_Returns_Expected_Value()
    {
        PageExtensions.GetServiceName().Should().Be(DefaultServiceName);
    }

    [Theory(DisplayName = $"{nameof(PageExtensions)} Data Tests")]
    [InlineData(true, null, DefaultServiceName)]
    [InlineData(true, "", DefaultServiceName)]
    [InlineData(true, DefaultServiceName + "", DefaultServiceName)]
    [InlineData(true, "Page Title", "Page Title | " + DefaultServiceName)]
    [InlineData(false, null, "Error: " + DefaultServiceName)]
    [InlineData(false, "", "Error: " + DefaultServiceName)]
    [InlineData(false, DefaultServiceName + "", "Error: " + DefaultServiceName)]
    [InlineData(false, "Page Title", "Error: Page Title | " + DefaultServiceName)]
    public void DataTests(bool isValid, string title, string expectedResult)
    {
        var generatedTitle = PageExtensions.GenerateTitle(title, isValid);
        generatedTitle.Should().Be(expectedResult);
    }

    [Theory(DisplayName = $"{nameof(PageExtensions)} Data Tests")]
    [InlineData(null, DefaultServiceName)]
    [InlineData("", DefaultServiceName)]
    [InlineData(DefaultServiceName + "", DefaultServiceName)]
    [InlineData("Page Title", "Page Title | " + DefaultServiceName)]
    public void DataTestsWithDefaultValue(string title, string expectedResult)
    {
        var generatedTitle = PageExtensions.GenerateTitle(title);

        generatedTitle.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatTitleWithAdministratorTag_Null()
    {
        const string title = "Test";
        const string expectedResult = "Test";

        var generatedTitle = PageExtensions.FormatTitleWithAdministratorTag(title, null);

        generatedTitle.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatTitleWithAdministratorTag_When_User_Has_No_Claims()
    {
        const string title = "Test";
        const string expectedResult = "Test";

       var generatedTitle = PageExtensions.FormatTitleWithAdministratorTag(
            title,
            new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>())));

        generatedTitle.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatTitleWithAdministratorTag_When_User_Has_UkPrn()
    {
        const string title = "Test";
        const string expectedResult = "Test";

        var generatedTitle = PageExtensions.FormatTitleWithAdministratorTag(
            title,
            new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new(CustomClaimTypes.UkPrn, "10000001")
                })));

        generatedTitle.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatTitleWithAdministratorTag_When_User_Is_Administrator_With_UkPrn()
    {
        const string title = "Test";
        const string expectedResult = "Test";
        
        var generatedTitle = PageExtensions.FormatTitleWithAdministratorTag(
            title,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(CustomClaimTypes.UkPrn, "10000001"),
                        new(ClaimTypes.Role, CustomRoles.Administrator)
                    })));

        generatedTitle.Should().Be(expectedResult);
    }

    [Fact]
    public void _FormatTitleWithAdministratorTag_When_User_Is_Administrator_Without_UkPrn()
    {
        const string title = "Test";
        const string expectedResult = "Test - Administrator";
        
        var generatedTitle = PageExtensions.FormatTitleWithAdministratorTag(
            title,
            new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimTypes.Role, CustomRoles.Administrator)
                })));

        generatedTitle.Should().Be(expectedResult);
    }
}