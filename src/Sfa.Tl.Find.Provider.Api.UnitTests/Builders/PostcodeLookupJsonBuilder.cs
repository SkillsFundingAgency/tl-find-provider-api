using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    // ReSharper disable StringLiteralTypo
    public static class PostcodeLookupJsonBuilder
    {
        public static string BuildValidPostcodeResponse() => 
            BuildResponse("validpostcoderesponse");

        public static string BuildValidPostcodeResponseWithNullLatLong() => 
            BuildResponse("validpostcodewithnulllatlongresponse");

        public static string BuildValidOutcodeResponse() => 
            BuildResponse("validoutcoderesponse");

        public static string BuildValidOutcodeResponseWithNullLatLong() => 
            BuildResponse("validoutcodewithnulllatlongresponse");

        public static string BuildTerminatedPostcodeResponse() => 
            BuildResponse("terminatedpostcoderesponse");

        public static string BuildValidTerminatedPostcodeResponseWithNullLatLong() => 
            BuildResponse("terminatedpostcodewithnulllatlongresponse");

        public static string BuildInvalidPostcodeResponse() => 
            BuildResponse("invalidpostcoderesponse");

        public static string BuildPostcodeNotFoundResponse() => 
            BuildResponse("postcodenotfoundresponse");
        
        private static string BuildResponse(string assetName) =>
            typeof(PostcodeLookupJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    $"Assets.{assetName}.json");
    }
}
