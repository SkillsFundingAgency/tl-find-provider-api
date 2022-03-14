using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

// ReSharper disable StringLiteralTypo
public static class PostcodeLookupJsonBuilder
{
    public static string BuildValidPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("validpostcoderesponse");

    public static string BuildValidPostcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("validpostcodewithnulllatlongresponse");

    public static string BuildValidOutcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("validoutcoderesponse");

    public static string BuildValidOutcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("validoutcodewithnulllatlongresponse");

    public static string BuildTerminatedPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("terminatedpostcoderesponse");

    public static string BuildValidTerminatedPostcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("terminatedpostcodewithnulllatlongresponse");

    public static string BuildInvalidPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("invalidpostcoderesponse");

    public static string BuildNearestPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("nearestpostcoderesponse");

    public static string BuildPostcodeNotFoundResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream("postcodenotfoundresponse");
}