using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

// ReSharper disable StringLiteralTypo
public static class NationalStatisticsJsonBuilder
{
    public static string BuildNationalStatisticsLocationsResponse() =>
        typeof(NationalStatisticsJsonBuilder).BuildJsonFromResourceStream("nationalstatisticslocationsresponse");
}