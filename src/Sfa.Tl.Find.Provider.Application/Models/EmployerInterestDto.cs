using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("UKPRN {" + nameof(OrganisationName) + "}" +
                 " {" + nameof(UniqueId) + ", nq}")]
public class EmployerInterestDto
{
    public Guid UniqueId { get; set; }
    public string OrganisationName { get; init; }
    public string ContactName { get; init; }
    public string Postcode { get; init; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool HasMultipleLocations { get; init; }
    public int LocationCount { get; init; }
    public int IndustryId { get; init; }
    public string OtherIndustry { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public string Website { get; init; }
    public int ContactPreferenceType { get; init; }
    public string AdditionalInformation { get; init; }
}
