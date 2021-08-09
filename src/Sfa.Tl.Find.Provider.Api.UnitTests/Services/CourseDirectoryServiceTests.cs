using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
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
            var validQualifications = new PostcodeLocationBuilder().BuildValidPostcodeLocation();

            var getTLevelDefinitionsUriFragment = "tleveldefinitions";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelDefinitionsUriFragment, jsonBuilder.BuildValidTLevelDefinitionsResponse() }
            };

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses);

            var result = await service.ImportQualifications();

            result.Saved.Should().Be(16);
            result.Deleted.Should().Be(0);
            //Verify(result, );
        }

        [Fact]
        public async Task ImportProviders_Returns_Expected_Result()
        {
            var getTLevelCoursesFragment = "tlevels";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelCoursesFragment, jsonBuilder.BuildValidTLevelsResponse() }
            };

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses);

            var result = await service.ImportProviders();

            result.Saved.Should().Be(0);
            result.Deleted.Should().Be(0);
        }
    }
}
