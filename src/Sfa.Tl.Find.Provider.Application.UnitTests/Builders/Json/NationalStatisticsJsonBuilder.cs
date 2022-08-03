using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Json;

// ReSharper disable StringLiteralTypo
public static class NationalStatisticsJsonBuilder
{
    private const string AssetFolderPath = "Assets.NationalStatistics";

    public static string BuildNationalStatisticsLocationsResponse() =>
        typeof(NationalStatisticsJsonBuilder).BuildJsonFromResourceStream(
            AssetFolderPath, 
            "nationalstatisticslocationsresponse");
}