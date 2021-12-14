using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class RouteBuilder
{
    public IEnumerable<Route> BuildList() =>
        new List<Route>
        {
            new() { Id = 1, Name = "Agriculture, environment and animal care" },
            new() { Id = 2, Name = "Business and administration" },
            new() { Id = 3, Name = "Catering" },
            new() { Id = 4, Name = "Construction" },
            new() { Id = 5, Name = "Creative and design" },
            new() { Id = 6, Name = "Digital and IT" },
            new() { Id = 7, Name = "Education and childcare" },
            new() { Id = 8, Name = "Engineering and manufacturing" },
            new() { Id = 9, Name = "Hair and beauty" },
            new() { Id = 10, Name = "Health and science" },
            new() { Id = 11, Name = "Legal, finance and accounting" }
        };
}