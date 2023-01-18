using System.Collections.ObjectModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders.HealthChecks;
public class HealthReportBuilder
{
    private const string AssetFolderPath = "Assets";

    public HealthReport Build()
    {
        var testData = new Dictionary<string, object>
        {
            {"test int", 1},
            {"test str", "hello"}
        };

        var result = new HealthReport(
            new ReadOnlyDictionary<string, HealthReportEntry>(
                new Dictionary<string, HealthReportEntry>
                {
                    {
                        "Info",
                        new HealthReportEntry(HealthStatus.Degraded, "Not great", TimeSpan.FromSeconds(5), new AccessViolationException(), testData)
                    }
                }),
            HealthStatus.Healthy, TimeSpan.MaxValue);

        return result;
    }

    public string BuildHealthReportJson() =>
        typeof(HealthReportBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "health");
}
