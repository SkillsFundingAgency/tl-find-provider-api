using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay("{" + nameof(Postcode) + "}" +
                     " {" + nameof(Distance) + ", nq}")]
    public class ProviderSearchResult
    {
        public long UkPrn { get; init; }
        public string ProviderName { get; init; }
        public string LocationName { get; init; }
        public string Postcode { get; init; }
        public string AddressLine1 { get; init; }
        public string AddressLine2 { get; init; }
        public string Town { get; init; }
        public string County { get; init; }
        public string Email { get; init; }
        public string Telephone { get; init; }
        public string Website { get; init; }
        public double Distance { get; init; }
        public string JourneyToLink { get; set; }

        public ICollection<DeliveryYear> DeliveryYears { get; init; } = new List<DeliveryYear>();
    }
}
