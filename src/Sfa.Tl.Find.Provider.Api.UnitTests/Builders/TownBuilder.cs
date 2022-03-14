﻿using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class TownBuilder
{
    public IEnumerable<Town> BuildList() =>
        new List<Town>
        {
            new()
            {
                Name = "Coventry",
                County = "West Midlands",
                LocalAuthorityName = "West Midlands",
                Latitude = 52.41695M,
                Longitude = -1.50721M
            },
            new()
            {
                Name = "Oxford",
                County = "Oxfordshire",
                LocalAuthorityName = "Oxfordshire",
                Latitude = 51.740811M,
                Longitude = -1.217524M
            }
        };
}