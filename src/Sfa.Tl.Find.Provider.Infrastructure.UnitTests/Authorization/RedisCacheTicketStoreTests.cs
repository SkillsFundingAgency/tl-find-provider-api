using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Authorization;
public class RedisCacheTicketStoreTests
{
    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(RedisCacheTicketStore)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }
}
