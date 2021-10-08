using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Filters
{
    public class HmacAuthorizationFilterTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(HmacAuthorizationFilter)
                .ShouldNotAcceptNullConstructorArguments();
        }
    }
}
