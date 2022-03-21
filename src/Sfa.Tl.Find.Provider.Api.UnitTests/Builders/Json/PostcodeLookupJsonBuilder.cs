using Sfa.Tl.Find.Provider.Api.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Json;

// ReSharper disable StringLiteralTypo
public static class PostcodeLookupJsonBuilder
{
    private const string AssetFolderPath = "Assets.Postcodes";

    public static string BuildValidPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "validpostcoderesponse");

    public static string BuildValidPostcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "validpostcodewithnulllatlongresponse");

    public static string BuildValidOutcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "validoutcoderesponse");

    public static string BuildValidOutcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath,
                "validoutcodewithnulllatlongresponse");

    public static string BuildTerminatedPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "terminatedpostcoderesponse");

    public static string BuildValidTerminatedPostcodeResponseWithNullLatLong() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "terminatedpostcodewithnulllatlongresponse");

    public static string BuildInvalidPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, 
                "invalidpostcoderesponse");

    public static string BuildNearestPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, "nearestpostcoderesponse");

    public static string BuildNullPostcodeResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, "nullpostcoderesponse");

    public static string BuildPostcodeNotFoundResponse() =>
        typeof(PostcodeLookupJsonBuilder)
            .BuildJsonFromResourceStream(
                AssetFolderPath, "postcodenotfoundresponse");
}