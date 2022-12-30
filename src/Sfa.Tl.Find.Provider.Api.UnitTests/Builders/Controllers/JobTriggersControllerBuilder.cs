using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Controllers;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class JobTriggersControllerBuilder
{
    public JobTriggersController Build(
        ISchedulerFactory schedulerFactory = null,
        ILogger<JobTriggersController> logger = null)
    {
        schedulerFactory ??= Substitute.For<ISchedulerFactory>();
        logger ??= Substitute.For<ILogger<JobTriggersController>>();

        var controller = new JobTriggersController(
            schedulerFactory, 
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