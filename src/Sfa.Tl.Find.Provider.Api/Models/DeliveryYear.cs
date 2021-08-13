using System.Collections.Generic;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    public class DeliveryYear
    {
        public short Year { get; init; }

        public ICollection<Qualification> Qualifications { get; init; }
    }
}
