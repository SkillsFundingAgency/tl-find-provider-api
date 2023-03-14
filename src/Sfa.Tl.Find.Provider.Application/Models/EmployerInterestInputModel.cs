using System.Diagnostics;
using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("{" + nameof(OrganisationName) + ", nq}")]
public class EmployerInterestInputModel
{
    public int Id { get; init; }
    public string OrganisationName { get; init; }
    public string ContactName { get; init; }
    public int? IndustryId { get; init; }
    public string OtherIndustry { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public ContactPreference? ContactPreferenceType { get; init; }
    public string AdditionalInformation { get; init; }
    public List<int>? SkillAreaIds { get; init; }
    public List<NamedLocation>? Locations { get; init; }
}
