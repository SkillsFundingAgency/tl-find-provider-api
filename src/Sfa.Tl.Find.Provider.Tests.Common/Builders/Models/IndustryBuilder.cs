using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class IndustryBuilder
{
    public string[] IndustryNames = 
    {
        "Admin and business support",
        "Agriculture",
        "Construction",
        "Retail and wholesale",
        "Health and social care",
        "Education and childcare",
        "Transport",
        "Utilities",
        "Manufacturing and engineering",
        "Arts, entertainment, and recreation",
        "Financial services",
        "Public sector",
        "IT and communications",
        "Hospitality",
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