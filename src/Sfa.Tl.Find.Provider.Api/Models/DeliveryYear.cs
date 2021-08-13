using System.Collections.Generic;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class DeliveryYear
    {
        public short Year { get; init; }

        public ICollection<Qualification> Qualifications { get; init; }

        private string DebuggerDisplay()
            => $"{Year} " +
               $"{(Qualifications != null ? Qualifications.Count : "null")} Qualifications";
    }
}
