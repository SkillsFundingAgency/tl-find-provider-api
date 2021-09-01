using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions
{
    public static class ProblemDetailExtensions
    {
        public static async Task ValidateProblemDetails(this HttpContent content, params (string FieldName, string ErrorMessage)[] expectedErrors)
        {
            try
            {
                var s = await content.ReadAsStringAsync();
                if (s.Length == 0)
                { }
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(
                    await content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true
                    });

                problemDetails.Should().NotBeNull();
                problemDetails!.Status.Should().Be((int)HttpStatusCode.BadRequest);
                problemDetails!.Extensions.Should().NotBeNullOrEmpty();
                problemDetails!.Extensions.Should().ContainKey("errors");

                var errors = JsonDocument.Parse(problemDetails!.Extensions["errors"]!.ToString() ?? string.Empty);

                foreach (var expectedError in expectedErrors)
                {
                    var pageError = errors.RootElement.GetProperty(expectedError.FieldName);
                    pageError.EnumerateArray().First().GetString().Should().Be(expectedError.ErrorMessage);
                }

            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
