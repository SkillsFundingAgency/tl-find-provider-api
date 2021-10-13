using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data
{
    public class DbContextWrapperTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(DbContextWrapper)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void GetQualifications_Returns_Expected_List()
        {
            var dbContextWrapper = new DbContextWrapperBuilder().Build();

            var connection = dbContextWrapper.CreateConnection();
            connection.Should().NotBeNull();
        }
    }
}
