using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    // ReSharper disable StringLiteralTypo
    public class PostcodeLookupJsonBuilder
    {
        public string BuildValidPostcodeResponse()
        {
            return GetAsset("validpostcoderesponse.json");
        }

        public string BuildInvalidPostcodeResponse()
        {
            return GetAsset("invalidpostcoderesponse.json");
        }

        public string BuildPostcodeNotFoundResponse()
        {
            return GetAsset("postcodenotfoundresponse.json");
        }

        public string BuildTerminatedPostcodeResponse()
        {
            return GetAsset("terminatedpostcoderesponse.json");
        }

        private string GetAsset(string assetName)
        {
            return $"{GetType().Namespace}.Assets.{assetName}"
                .ReadManifestResourceStreamAsString();
        }
    }
}
