using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class DataImportControllerBuilder
{
    public DataImportController Build(
        ITownDataService townDataService = null,
        ILogger<DataImportController> logger = null)
    {
        townDataService ??= Substitute.For<ITownDataService>();
        logger ??= Substitute.For<ILogger<DataImportController>>();

        var controller = new DataImportController(
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
