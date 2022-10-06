using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class IndustriesControllerBuilder
{
    public IndustriesController Build(
        IProviderDataService providerDataService = null,
        ILogger<IndustriesController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<IndustriesController>>();

        var controller = new IndustriesController(
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