using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                     " {" + nameof(Postcode) + "}" +
                     " {" + nameof(DeliveryYear) + ", nq}" +
                     " { Qualification" + nameof(QualificationId) + ", nq}")]
    public class LocationQualificationDto
    {
        public long UkPrn { get; init; }
        public string Postcode { get; init; }
        public  short DeliveryYear { get; init; }
        public  int QualificationId { get; init; }
    }
}
