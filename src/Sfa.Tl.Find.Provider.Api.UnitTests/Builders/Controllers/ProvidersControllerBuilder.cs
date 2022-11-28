using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class ProvidersControllerBuilder
{
    public ProvidersController Build(
        IProviderDataService providerDataService = null,
        IDateTimeProvider dateTimeProvider = null,
        ICacheService cacheService = null,
        ILogger<ProvidersController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        dateTimeProvider ??= Substitute.For<IDateTimeProvider>();
        cacheService ??= Substitute.For<ICacheService>();
        logger ??= Substitute.For<ILogger<ProvidersController>>();

        var controller = new ProvidersController(
            providerDataService,
            dateTimeProvider,
            cacheService,
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