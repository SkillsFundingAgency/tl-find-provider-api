using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Application.HealthChecks;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.HealthChecks;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.HealthChecks;
public class HealthCheckResponseWriterTests
{
    [Fact]
    public async Task HealthCheckResponseWriter_Returns_Expected_Response()
    {
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var builder = new HealthReportBuilder();
        var healthReport = builder.Build();
        var expectedJson = builder.BuildHealthReportJson();

        var task = HealthCheckResponseWriter
            .WriteJsonResponse(httpContext, healthReport);

        task.Should().NotBeNull();
        //await task;
        task.GetAwaiter().GetResult();

        //task.GetAwaiter().GetResult();

        httpContext.Response.Should().NotBeNull();

        httpContext.Response.Body.CanRead.Should().BeTrue();
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(httpContext.Response.Body);
        var response = await reader.ReadToEndAsync();

        response.Should().Be(expectedJson);
        
    }
}
