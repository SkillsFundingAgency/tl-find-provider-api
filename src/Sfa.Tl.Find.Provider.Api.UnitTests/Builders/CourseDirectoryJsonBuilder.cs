using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

// ReSharper disable StringLiteralTypo
public static class CourseDirectoryJsonBuilder
{
    public static string BuildValidTLevelDefinitionsResponse() => 
        BuildResponse("tleveldefinitions");

    public static string BuildValidTLevelsResponse() => 
        BuildResponse("tlevels");

    private static string BuildResponse(string assetName) =>
        typeof(CourseDirectoryJsonBuilder)
            .ReadManifestResourceStreamAsString(
                $"Assets.{assetName}.json");
}