using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    // ReSharper disable StringLiteralTypo
    public class PostcodeLookupJsonBuilder
    {
        public string BuildValidPostcodeResponse()
        {
            return typeof(PostcodeLookupJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.validpostcoderesponse.json");
        }

        public string BuildInvalidPostcodeResponse()
        {
            return typeof(PostcodeLookupJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.invalidpostcoderesponse.json");
        }

        public string BuildPostcodeNotFoundResponse()
        {
            return typeof(PostcodeLookupJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.postcodenotfoundresponse.json");
        }

        public string BuildTerminatedPostcodeResponse()
        {
            return typeof(PostcodeLookupJsonBuilder)
                .ReadManifestResourceStreamAsString(
                    "Assets.terminatedpostcoderesponse.json");
        }
    }
}
