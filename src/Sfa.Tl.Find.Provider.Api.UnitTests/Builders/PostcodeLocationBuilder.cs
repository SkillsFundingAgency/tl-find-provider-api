using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class PostcodeLocationBuilder
    {
        public PostcodeLocation BuildValidPostcodeLocation() =>
            new()
            {
                Postcode = "CV1 2WT",
                Latitude = 52.400997,
                Longitude = -1.508122
            };

        public PostcodeLocation BuildInvalidPostcodeLocation() =>
            new()
            {
                Postcode = "CV99 XXX",
                Latitude = double.NaN,
                Longitude = double.NaN
            };

        public PostcodeLocation BuildTerminatedPostcodeLocation() =>
            new()
            {
                Postcode = "S70 2YW",
                Latitude = 53.551618,
                Longitude = -1.482797
            };

        public PostcodeLocation BuildPostcodeLocation(
            string postcode = "CV1 2WT") =>
            new()
            {
                Postcode = postcode,
                Latitude = 50.0,
                Longitude = -1.0
            };
    }
}
