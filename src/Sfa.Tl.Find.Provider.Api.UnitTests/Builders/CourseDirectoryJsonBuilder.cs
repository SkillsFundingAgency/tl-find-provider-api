using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

// ReSharper disable StringLiteralTypo
public static class CourseDirectoryJsonBuilder
{
    public static string BuildValidTLevelDefinitionsResponse() =>
        typeof(CourseDirectoryJsonBuilder)
            .BuildJsonFromResourceStream("tleveldefinitions");

    public static string BuildValidTLevelsResponse() =>
        typeof(CourseDirectoryJsonBuilder)
            .BuildJsonFromResourceStream("tlevels");
}