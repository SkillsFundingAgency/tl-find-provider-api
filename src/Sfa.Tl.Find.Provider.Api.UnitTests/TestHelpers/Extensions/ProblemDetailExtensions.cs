﻿using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class ProblemDetailExtensions
{
    public static async Task ValidateProblemDetails(this HttpContent content, params (string FieldName, string ErrorMessage)[] expectedErrors)
    {
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

        foreach (var (fieldName, errorMessage) in expectedErrors)
        {
            var error = errors.RootElement.GetProperty(fieldName);
            error.EnumerateArray().First().GetString().Should().Be(errorMessage);
        }
    }
}