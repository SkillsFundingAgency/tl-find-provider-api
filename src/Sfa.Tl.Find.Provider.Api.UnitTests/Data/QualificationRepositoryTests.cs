using System.Threading.Tasks;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestEHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data
{
    public class QualificationRepositoryTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(QualificationRepository)
                .ShouldNotAcceptNullConstructorArguments();
        }
        
        [Fact]
        public async Task GetQualifications_Returns_Expected_List()
        {
            var repository = new QualificationRepositoryBuilder().Build();

            var results = await repository.GetAllQualifications();
            results.Should().NotBeNullOrEmpty();
        }
    }
}
