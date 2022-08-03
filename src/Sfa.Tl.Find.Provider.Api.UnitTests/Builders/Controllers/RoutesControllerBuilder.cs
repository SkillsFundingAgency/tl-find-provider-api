using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class RoutesControllerBuilder
{
    public RoutesController Build(
        IProviderDataService providerDataService = null,
        ILogger<RoutesController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<RoutesController>>();

        var controller = new RoutesController(
            providerDataService, 
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