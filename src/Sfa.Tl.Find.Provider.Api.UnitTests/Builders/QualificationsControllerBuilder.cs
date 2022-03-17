using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders;

public class QualificationsControllerBuilder
{
    public QualificationsController Build(
        IProviderDataService providerDataService = null,
        ILogger<QualificationsController> logger = null)
    {
        providerDataService ??= Substitute.For<IProviderDataService>();
        logger ??= Substitute.For<ILogger<QualificationsController>>();

        var controller = new QualificationsController(
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