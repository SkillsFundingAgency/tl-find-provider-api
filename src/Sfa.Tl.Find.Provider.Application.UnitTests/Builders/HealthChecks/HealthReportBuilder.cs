﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.ObjectModel;
using Sfa.Tl.Find.Provider.Application.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.HealthChecks;
public class HealthReportBuilder
{
    private const string AssetFolderPath = "Assets";

    public HealthReport Build()
    {
        var testData = new Dictionary<string, object>
        {
            {"test int", 1},
            {"test str", "hello"},
            //{"test obj", new {i = 1, s = "x"}},
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
