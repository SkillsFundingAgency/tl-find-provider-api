using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class IndustryBuilder
{
    public IEnumerable<Industry> BuildList()
    {
        return new List<Industry>
        {
            new()
            {
                Id = 1,
                Name = "Agriculture, Environment and Animal Care",
                ShortName = "Agriculture"
            },
            new()
            {
                Id = 2,
                Name = "Business and Administration",
                ShortName = "Business",
            },
            new()
            {
                Id = 3,
                Name = "Catering",
                ShortName = "Catering"
            },
            new()
            {
                Id = 4,
                Name = "Construction and the Built Environment",
                ShortName = "Construction"
            },
            new()
            {
                Id = 5,
                Name = "Creative and Design",
                ShortName = "Creative and Design"
            },
            new()
            {
                Id = 6,
                Name = "Digital and IT",
                ShortName = "Digital"
            },
            new()
            {
                Id = 7,
                Name = "Education and Childcare",
                ShortName = "Education"
            },
            new()
            {
                Id = 8,
                Name = "Engineering and Manufacturing",
                ShortName = "Engineering"
            },
            new()
            {
                Id = 9,
                Name = "Hair and Beauty",
                ShortName = "Hair and Beauty"
            },
            new()
            {
                Id = 10,
                Name = "Health and Science",
                ShortName = "Health and Science"
            },
            new()
            {
                Id = 11,
                Name = "Legal, Finance and Accounting",
                ShortName = "Legal, Finance and Accounting"
            }
        };
    }
}