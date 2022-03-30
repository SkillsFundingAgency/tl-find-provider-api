﻿using System.Collections.Generic;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Models;

public class RouteBuilder
{
    public IEnumerable<Route> BuildList() =>
        new List<Route>
        {
            new() 
            {
                Id = 1,
                Name = "Agriculture, environment and animal care",
                NumberOfQualifications = 1,
                NumberOfQualificationsOffered = 11
            },
            new() 
            { 
                Id = 2,
                Name = "Business and administration", 
                NumberOfQualifications = 2,
                NumberOfQualificationsOffered = 21
            },
            new() {
                Id = 3,
                Name = "Catering",
                NumberOfQualifications = 3,
                NumberOfQualificationsOffered = 31
            },
            new() {
                Id = 4,
                Name = "Construction",
                NumberOfQualifications= 4,
                NumberOfQualificationsOffered = 41
            },
            new() 
            { 
                Id = 5, 
                Name = "Creative and design",
                NumberOfQualifications= 5,
                NumberOfQualificationsOffered = 51
            },
            new()
            {
                Id = 6, 
                Name = "Digital and IT",
                NumberOfQualifications= 6,
                NumberOfQualificationsOffered = 61
            },
            new()
            {
                Id = 7, 
                Name = "Education and childcare",
                NumberOfQualifications= 7,
                NumberOfQualificationsOffered = 71
            },
            new()
            {
                Id = 8, 
                Name = "Engineering and manufacturing",
                NumberOfQualifications= 8,
                NumberOfQualificationsOffered = 81
            },
            new()
            {
                Id = 9, 
                Name = "Hair and beauty",
                NumberOfQualifications= 9,
                NumberOfQualificationsOffered = 91
            },
            new()
            {
                Id = 10, 
                Name = "Health and science",
                NumberOfQualifications= 10,
                NumberOfQualificationsOffered = 101
            },
            new()
            {
                Id = 11, 
                Name = "Legal, finance and accounting",
                NumberOfQualifications= 11,
                NumberOfQualificationsOffered = 111            }
        };
}