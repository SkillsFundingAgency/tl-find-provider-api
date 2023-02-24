using System.Diagnostics;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Name) + ", nq}")]
public class Provider
{
    public long UkPrn { get; init; }
    public string Name { get; init; }
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; init; }
    public string Town { get; init; }
    public string County { get; init; }
    public string Postcode { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public string EmployerContactEmail { get; init; }
    public string EmployerContactTelephone { get; init; }
    public string EmployerContactWebsite { get; init; }
    public string StudentContactEmail { get; init; }
    public string StudentContactTelephone { get; init; }
    public string StudentContactWebsite { get; init; }

    [Write(false)]
    public ICollection<Location> Locations { get; init; }
}