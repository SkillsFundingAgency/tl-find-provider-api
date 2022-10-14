using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class DataImportControllerBuilder
{
    public DataImportController Build(
        IProviderDataService providerDataService = null,
        ILogger<DataImportController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<DataImportController>>();

        var controller = new DataImportController(
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
