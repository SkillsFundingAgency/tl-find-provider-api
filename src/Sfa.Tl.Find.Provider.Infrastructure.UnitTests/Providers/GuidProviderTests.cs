using Sfa.Tl.Find.Provider.Infrastructure.Providers;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Providers;

public class GuidProviderTests
{
    [Fact]
    public void GuidProvider_Generates_New_Guid()
    {
        var result = new GuidProvider().NewGuid();

        result.Should().NotBe(Guid.Empty);
    }
}
