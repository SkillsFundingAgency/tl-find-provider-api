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

    [Theory(DisplayName = "PageExtensions Data Tests")]
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

    [Theory(DisplayName = "PageExtensions Data Tests")]
    [InlineData(null, DefaultServiceName)]
    [InlineData("", DefaultServiceName)]
    [InlineData(DefaultServiceName + "", DefaultServiceName)]
    [InlineData("Page Title", "Page Title | " + DefaultServiceName)]
    public void DataTestsWithDefaultValue(string title, string expectedResult)
    {
        var generatedTitle = PageExtensions.GenerateTitle(title);

        generatedTitle.Should().Be(expectedResult);
    }
}