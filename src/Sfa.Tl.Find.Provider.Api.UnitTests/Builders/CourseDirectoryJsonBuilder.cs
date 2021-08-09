using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    // ReSharper disable StringLiteralTypo
    public class CourseDirectoryJsonBuilder
    {
        public string BuildValidTLevelDefinitionsResponse()
        {
            return typeof(CourseDirectoryJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.tleveldefinitions.json");
        }

        public string BuildValidTLevelsResponse()
        {
            return typeof(CourseDirectoryJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.tlevels.json");
        }
    }
}
