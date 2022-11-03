using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions;

public class SecurityHeaderExtensionsTests
{
    [Fact]
    public void GetHeaderPolicyCollection_Returns_Non_Empty_Collection_For_Dev()
    {
        var headerCollection = SecurityHeaderExtensions.GetHeaderPolicyCollection(true);
        headerCollection.Should().NotBeNull();
        headerCollection.Should().NotBeEmpty();
    }

    [Fact]
    public void GetHeaderPolicyCollection_Returns_Non_Empty_Collection_For_Non_Dev()
    {
        var headerCollection = SecurityHeaderExtensions.GetHeaderPolicyCollection(false);
        headerCollection.Should().NotBeNull();
        headerCollection.Should().NotBeEmpty();
    }
}