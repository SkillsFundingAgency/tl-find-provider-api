using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class IndustryBuilder
{
    public string[] IndustryNames = 
    {
        "Admin and Business Support",
        "Agriculture",
        "Arts, Entertainment and Recreation",
        "Construction",
        "Education and Childcare",
        "Financial Services",
        "Health and Social Care",
        "Hospitality",
        "IT and Communications",
        "Manufacturing and Engineering",
        "Public Sector",
        "Retail and Wholesale",
        "Transport",
        "Utilities",
        "Other"
    };

    public IEnumerable<Industry> BuildList() =>
        IndustryNames
            .Select(
                (n, i) => 
                    new Industry
                    {
                        Id = i + 1, 
                        Name = n
                    }).ToList();
}