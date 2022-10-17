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
}