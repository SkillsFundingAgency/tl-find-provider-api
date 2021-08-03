using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay(" {" + nameof(Postcode) + ", nq}" +
                     " ({" + nameof(Latitude) + "}, {" + nameof(Longitude) + "})")]
    public class PostcodeLocation
    {
        public string Postcode { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }
}
