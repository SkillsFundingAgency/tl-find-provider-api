using System.Diagnostics;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(OrganisationName) + "}" +
                 " {" + nameof(UniqueId) + ", nq}")]
public class EmployerInterest
{
    public int Id { get; init; }
    public Guid UniqueId { get; set; }
    public string OrganisationName { get; init; }
    public string ContactName { get; init; }
    public string Postcode { get; init; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? IndustryId { get; init; }
    public string OtherIndustry { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public int ExtensionCount { get; init; }
    public ContactPreference? ContactPreferenceType { get; init; }
    public string AdditionalInformation { get; init; }
    public List<int>? SkillAreaIds { get; init; }
}
