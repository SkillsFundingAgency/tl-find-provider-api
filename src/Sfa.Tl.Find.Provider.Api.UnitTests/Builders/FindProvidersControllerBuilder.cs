﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class FindProvidersControllerBuilder
{
    public FindProvidersController Build(
        IProviderDataService providerDataService = null,
        ITownDataService townDataService = null,
        ILogger<FindProvidersController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<FindProvidersController>>();

        var controller = new FindProvidersController(
            providerDataService, 
            townDataService, 
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