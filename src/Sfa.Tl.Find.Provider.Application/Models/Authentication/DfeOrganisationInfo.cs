using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models.Authentication;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class DfeOrganisationInfo
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public long? UkPrn { get; set; }

    public long? Urn { get; set; }
}
