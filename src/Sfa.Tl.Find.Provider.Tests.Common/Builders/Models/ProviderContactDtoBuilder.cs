using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class ProviderContactDtoBuilder
{
    public ProviderContactDto Build() =>
        new()
        {
            UkPrn = 10000055,
            Name = "ABINGDON AND WITNEY COLLEGE",
            EmployerContactEmail = "employer.guidance@abingdon-witney.ac.uk",
            EmployerContactTelephone = "01235 789010",
            EmployerContactWebsite = "http://www.abingdon-witney.ac.uk/employers",
            StudentContactEmail = "student.counseller@abingdon-witney.ac.uk",
            StudentContactTelephone = "01235 789010",
            StudentContactWebsite = "http://www.abingdon-witney.ac.uk/students"
        };
}