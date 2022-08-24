using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class ProvidersControllerBuilder
{
    public ProvidersController Build(
        IProviderDataService providerDataService = null,
        IDateTimeService dateTimeService = null,
        IMemoryCache cache = null,
        ILogger<ProvidersController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        dateTimeService ??= Substitute.For<IDateTimeService>();
        cache ??= Substitute.For<IMemoryCache>();
        logger ??= Substitute.For<ILogger<ProvidersController>>();

        var controller = new ProvidersController(
            providerDataService,
            dateTimeService, 
            cache,
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