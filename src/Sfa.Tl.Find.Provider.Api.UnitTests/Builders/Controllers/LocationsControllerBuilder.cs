﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class LocationsControllerBuilder
{
    public LocationsController Build(
        ITownDataService townDataService = null,
        IPostcodeLookupService postcodeLookupService = null,
        ILogger<LocationsController> logger = null)
    {
        townDataService ??= Substitute.For<ITownDataService>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        logger ??= Substitute.For<ILogger<LocationsController>>();

        var controller = new LocationsController(
            townDataService, 
            postcodeLookupService,
            logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}