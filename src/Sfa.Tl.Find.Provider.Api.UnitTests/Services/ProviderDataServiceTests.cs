using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestEHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services
{
    public class ProviderDataServiceTests
    {
        private const string TestPostCode = "AB1 2XY";
        
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(ProviderDataService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(ProviderDataService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task GetQualifications_Returns_Expected_List()
        {
            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository.GetAllQualifications()
                .Returns(new QualificationBuilder().BuildList().AsQueryable());

            var controller = new ProviderDataServiceBuilder()
                .Build(qualificationRepository: qualificationRepository);

            var results = await controller.GetQualifications();
            results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetProviders_Returns_Expected_List()
        {
            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.GetAllProviders()
                .Returns(new ProviderBuilder().BuildList().AsQueryable());

            var service = new ProviderDataServiceBuilder().Build(providerRepository);

            var results = await service.FindProviders(TestPostCode);
            results.Should().NotBeNullOrEmpty();
        }
    }
}
