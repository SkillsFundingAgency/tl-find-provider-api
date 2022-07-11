using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;

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
                NumberOfQualificationsOffered = 11,
                Qualifications = qualificationBuilder.BuildList(1)
            },
            new()
            {
                Id = 2,
                Name = "Business and administration",
                NumberOfQualificationsOffered = 21,
                Qualifications = qualificationBuilder.BuildList(2)
            },
            new()
            {
                Id = 3,
                Name = "Catering",
                NumberOfQualificationsOffered = 31,
                Qualifications = qualificationBuilder.BuildList(3)
            },
            new()
            {
                Id = 4,
                Name = "Construction and the built environment",
                NumberOfQualificationsOffered = 41,
                Qualifications = qualificationBuilder.BuildList(4)
            },
            new()
            {
                Id = 5,
                Name = "Creative and design",
                NumberOfQualificationsOffered = 51,
                Qualifications = qualificationBuilder.BuildList(5)
            },
            new()
            {
                Id = 6,
                Name = "Digital and IT",
                NumberOfQualificationsOffered = 61,
                Qualifications = qualificationBuilder.BuildList(6)
            },
            new()
            {
                Id = 7,
                Name = "Education and childcare",
                NumberOfQualificationsOffered = 71,
                Qualifications = qualificationBuilder.BuildList(7)
            },
            new()
            {
                Id = 8,
                Name = "Engineering and manufacturing",
                NumberOfQualificationsOffered = 81,
                Qualifications = qualificationBuilder.BuildList(8)
            },
            new()
            {
                Id = 9,
                Name = "Hair and beauty",
                NumberOfQualificationsOffered = 91,
                Qualifications = qualificationBuilder.BuildList(9)
            },
            new()
            {
                Id = 10,
                Name = "Health and science",
                NumberOfQualificationsOffered = 101,
                Qualifications = qualificationBuilder.BuildList(10)
            },
            new()
            {
                Id = 11,
                Name = "Legal, finance and accounting",
                NumberOfQualificationsOffered = 111,
                Qualifications = qualificationBuilder.BuildList(11)
            }
        };
    }
}