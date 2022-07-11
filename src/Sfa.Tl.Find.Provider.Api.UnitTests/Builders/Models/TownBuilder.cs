using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;

public class TownBuilder
{
    public IEnumerable<Town> BuildList() =>
        new List<Town>
        {
            new()
            {
                Id = 1,
                Name = "Coventry",
                County = "West Midlands",
                LocalAuthority = "West Midlands",
                Latitude = 52.41695M,
                Longitude = -1.50721M
            },
            new()
            {
                Id = 2,
                Name = "Oxford",
                County = "Oxfordshire",
                LocalAuthority = "Oxfordshire",
                Latitude = 51.740811M,
                Longitude = -1.217524M
            }
        };

    public IEnumerable<Town> BuildListForStAgnes() =>
        new List<Town>
        {
            new()
            {
                Id = 1,
                Name = "St Agnes",
                County = null,
                LocalAuthority = "Cornwall",
                Latitude = 50.309347M,
                Longitude = -5.203885M
            },
            new()
            {
                Id = 2,
                Name = "St. Agnes",
                County = null,
                LocalAuthority = "Cornwall",
                Latitude = 50.275615M,
                Longitude = -5.205394M
            }
        };
}