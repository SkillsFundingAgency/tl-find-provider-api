using Sfa.Tl.Find.Provider.Infrastructure.Services;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class GuidServiceTests0
{
    [Fact]
    public void GuidService_Generates_New_Guid()
    {
        var result = new GuidService().NewGuid();
        
        result.Should().NotBe(Guid.Empty);
    }
}
