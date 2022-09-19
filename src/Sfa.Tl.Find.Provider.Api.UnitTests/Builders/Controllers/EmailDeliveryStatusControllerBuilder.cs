using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;

public class EmailDeliveryStatusControllerBuilder
{
    public EmailDeliveryStatusController Build(
        IEmailDeliveryStatusService emailDeliveryStatusService = null,
        EmailSettings emailSettings = null,
        ILogger<EmailDeliveryStatusController> logger = null)
    {
        emailDeliveryStatusService ??= Substitute.For<IEmailDeliveryStatusService>();
        logger ??= Substitute.For<ILogger<EmailDeliveryStatusController>>();

        emailSettings ??= new SettingsBuilder().BuildEmailSettings();
        var emailOptions = emailSettings.ToOptions();

        var controller = new EmailDeliveryStatusController(
            //emailService,
            emailDeliveryStatusService, 
            emailOptions,
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