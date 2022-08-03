using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

// ReSharper disable StringLiteralTypo
public static class CourseDirectoryJsonBuilder
{
    private const string AssetFolderPath = "Assets.CourseDirectory";

    public static string BuildValidTLevelDefinitionsResponse() =>
        typeof(CourseDirectoryJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "tleveldefinitions");

    public static string BuildValidTLevelsResponse() =>
        typeof(CourseDirectoryJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "tlevels");
}