using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Json;

public static class PayloadJsonBuilder
{
    private const string AssetFolderPath = "Assets";

    public static string BuildCreateEmployerInterestPayload() =>
        typeof(PayloadJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "createEmployerInterest");

    public static string BuildCreateEmployerInterestPayloadWithEmptyLocations() =>
        typeof(PayloadJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "createEmployerInterestWithEmptyLocations");

    public static string BuildCreateEmployerInterestPayloadWithNoLocations() =>
        typeof(PayloadJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "createEmployerInterestWithNoLocations");

    public static string BuildCreateEmployerInterestPayloadWithTwoLocations() =>
        typeof(PayloadJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "createEmployerInterestWithTwoLocations");
}