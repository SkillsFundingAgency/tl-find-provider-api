using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models.Authentication;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(Email) + ", nq}")]
public class DfeUserInfo
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public long? UkPrn { get; set; }

    public long? Urn { get; set; }

    public IEnumerable<Role> Roles { get; set; }
}
