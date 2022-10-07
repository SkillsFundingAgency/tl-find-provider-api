using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sfa.Tl.Find.Provider.Application.HealthChecks;
public static class HealthCheckResponseWriter
{
    public static Task WriteJsonResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";

        var entries = result.Entries.ToDictionary(
            d => d.Key,
            d => new
            {
                Status = d.Value.Status.ToString(),
                Duration = d.Value.Duration.TotalSeconds.ToString(NumberFormatInfo.InvariantInfo),
                d.Value.Description,
                d.Value.Data
            });

        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = result.Status.ToString(),
            results = result.Entries.ToDictionary(
                d => d.Key,
                              d => new
                              {
                                  Status = d.Value.Status.ToString(),
                                  Duration = d.Value.Duration.TotalSeconds.ToString(NumberFormatInfo.InvariantInfo),
                                  Description = d.Value.Description,
                                  Data = d.Value.Data
                              })
        }, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return httpContext.Response.WriteAsync(json);
    }

    public static HealthReport GetTestData()
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
}
