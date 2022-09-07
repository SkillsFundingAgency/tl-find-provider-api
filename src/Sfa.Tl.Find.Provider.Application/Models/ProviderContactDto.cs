using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class ProviderContactDto
{
    public long UkPrn { get; init; }
    public string Name { get; init; }
    public string EmployerContactEmail { get; init; }
    public string EmployerContactTelephone { get; init; }
    public string EmployerContactWebsite { get; init; }
    public string StudentContactEmail { get; init; }
    public string StudentContactTelephone { get; init; }
    public string StudentContactWebsite { get; init; }
}
