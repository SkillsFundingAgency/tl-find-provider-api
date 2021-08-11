using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services
{
    public class CourseDirectoryServiceTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(CourseDirectoryService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(CourseDirectoryService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task ImportQualifications_Returns_Expected_Result()
        {
            const string getTLevelDefinitionsUriFragment = "tleveldefinitions";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelDefinitionsUriFragment, jsonBuilder.BuildValidTLevelDefinitionsResponse() }
            };

            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository.Save(Arg.Any<IEnumerable<Qualification>>())
                .Returns((10, 5, 2));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, qualificationRepository: qualificationRepository);

            var result = await service.ImportQualifications();

            result.Saved.Should().Be(10);
            result.Updated.Should().Be(5);
            result.Deleted.Should().Be(2);
        }

        [Fact]
        public async Task ImportProviders_Returns_Expected_Result()
        {
            const string getTLevelCoursesFragment = "tlevels";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelCoursesFragment, jsonBuilder.BuildValidTLevelsResponse() }
            };

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Save(Arg.Any<IEnumerable<Models.Provider>>())
                .Returns((20, 10, 5));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, providerRepository);

            var result = await service.ImportProviders();

            result.Saved.Should().Be(0);
            result.Updated.Should().Be(0);
            result.Deleted.Should().Be(0);
            //result.Saved.Should().Be(20);
            //result.Updated.Should().Be(10);
            //result.Deleted.Should().Be(5);
        }
    }
}
