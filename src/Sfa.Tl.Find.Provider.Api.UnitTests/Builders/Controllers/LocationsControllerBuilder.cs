using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class LocationsControllerBuilder
{
    public LocationsController Build(
        ITownDataService townDataService = null,
        ILogger<LocationsController> logger = null)
    {
        townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<LocationsController>>();

        var controller = new LocationsController(
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