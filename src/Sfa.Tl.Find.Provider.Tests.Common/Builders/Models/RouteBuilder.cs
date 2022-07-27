using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class RouteBuilder
{
    public IEnumerable<Route> BuildList()
    {
        var qualificationBuilder = new QualificationBuilder();
        return new List<Route>
        {
            new()
            {
                Id = 1,
                Name = "Agriculture, environment and animal care",
                Qualifications = qualificationBuilder.BuildList(1)
            },
            new()
            {
                Id = 2,
                Name = "Business and administration",
                Qualifications = qualificationBuilder.BuildList(2)
            },
            new()
            {
                Id = 3,
                Name = "Catering",
                Qualifications = qualificationBuilder.BuildList(3)
            },
            new()
            {
                Id = 4,
                Name = "Construction and the built environment",
                Qualifications = qualificationBuilder.BuildList(4)
            },
            new()
            {
                Id = 5,
                Name = "Creative and design",
                Qualifications = qualificationBuilder.BuildList(5)
            },
            new()
            {
                Id = 6,
                Name = "Digital and IT",
                Qualifications = qualificationBuilder.BuildList(6)
            },
            new()
            {
                Id = 7,
                Name = "Education and childcare",
                Qualifications = qualificationBuilder.BuildList(7)
            },
            new()
            {
                Id = 8,
                Name = "Engineering and manufacturing",
                Qualifications = qualificationBuilder.BuildList(8)
            },
            new()
            {
                Id = 9,
                Name = "Hair and beauty",
                Qualifications = qualificationBuilder.BuildList(9)
            },
            new()
            {
                Id = 10,
                Name = "Health and science",
                Qualifications = qualificationBuilder.BuildList(10)
            },
            new()
            {
                Id = 11,
                Name = "Legal, finance and accounting",
                Qualifications = qualificationBuilder.BuildList(11)
            }
        };
    }
}