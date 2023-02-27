using Sfa.Tl.Find.Provider.Application.Models.Enums;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(OrganisationName) + "}" +
                 " {" + nameof(UniqueId) + ", nq}")]
public class EmployerInterestDetailDto
{
    public int Id { get; init; }
    public Guid UniqueId { get; init; }
    public string OrganisationName { get; init; }
    public string ContactName { get; init; }
    public string LocationName { get; init; }
    public string Postcode { get; init; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Industry { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public ContactPreference? ContactPreferenceType { get; init; }
    public string AdditionalInformation { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public int ExtensionCount { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ModifiedOn { get; init; }
}
