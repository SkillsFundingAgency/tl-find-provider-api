using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

// ReSharper disable StringLiteralTypo
public static class GoogleApiJsonBuilder
{
    private const string AssetFolderPath = "Assets.GoogleApi";

    public static string BuildValidResponse() =>
        typeof(GoogleApiJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "GooglePlacesResult");

    public static string BuildZeroResultsResponse() =>
        typeof(GoogleApiJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "GooglePlacesZeroResults");
}