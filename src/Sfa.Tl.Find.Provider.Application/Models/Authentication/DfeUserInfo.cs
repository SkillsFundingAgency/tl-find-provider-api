using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models.Authentication;

[DebuggerDisplay("UKPRN {" + nameof(UkPrn) + "}" +
                 " {" + nameof(DisplayName) + ", nq}")]
public class DfeUserInfo
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; }

    public string Surname { get; set; }

    public string DisplayName =>
        string.IsNullOrEmpty(FirstName) 
            ? Surname
            : string.IsNullOrEmpty(Surname)
                ? FirstName
                : $"{FirstName} {Surname}";

    public string Email { get; set; }

    public long? UkPrn { get; set; }

    public IEnumerable<Role> Roles { get; set; }

    public bool HasAccessToService { get; set; } = true;
}
