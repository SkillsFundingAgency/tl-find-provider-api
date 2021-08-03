using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                     " {" + nameof(Name) + ", nq}")]
    public class Provider
    {
        public long UkPrn { get; init; }
        public string Name { get; init; }
    }
}
