namespace Sfa.Tl.Find.Provider.Application.Models;
public class EmployerInterest
{
    public int Id { get; init; }
    public string UniqueId { get; init; }
    public string OrganisationName { get; init; }
    public string ContactName { get; init; }
    public string Postcode { get; init; }
    public bool HasMultipleLocations { get; init; }
    public int LocationCount { get; init; }
    public int IndustryId { get; init; }
    public string SpecificRequirements { get; init; }
    public string Email { get; init; }
    public string Telephone { get; init; }
    public int ContactPreferenceType { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? ModifiedOn { get; init; }
}
