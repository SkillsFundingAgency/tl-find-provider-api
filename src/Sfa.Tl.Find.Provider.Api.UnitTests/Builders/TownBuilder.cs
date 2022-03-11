using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class TownBuilder
{
    public IEnumerable<Town> BuildList() =>
        new List<Town>
        {
            new()
            {
                Id = 1,
                Name = "Coventry",
                County = "Coventry"
            },
            new()
            {
                Id = 2,
                Name = "Oxford",
                County = "Oxfordshire"
            }
        };
}