using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Web.Controllers;

[ApiController]
[Route("[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmailDeliveryStatusController : ControllerBase
{
    private readonly IEmailDeliveryStatusService _emailDeliveryStatusService;

    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailDeliveryStatusController> _logger;

    public EmailDeliveryStatusController(
        IEmailDeliveryStatusService emailDeliveryStatusService,
        IOptions<EmailSettings>? emailOptions,
        ILogger<EmailDeliveryStatusController> logger)
    {
        _emailDeliveryStatusService = emailDeliveryStatusService ?? throw new ArgumentNullException(nameof(emailDeliveryStatusService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("callback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EmailDeliveryStatusCallback(EmailDeliveryReceipt deliveryReceipt)
    {
        _logger.LogInformation("Email delivery callback received " +
                               "{type} status {status} reference {reference} template {templateId}",
            deliveryReceipt.NotificationType,
            deliveryReceipt.EmailDeliveryStatus,
            deliveryReceipt.Reference,
            deliveryReceipt.TemplateId);
        
        try
        {
            if(!(Request.Headers
                     .TryGetValue("Authorization", out var token)
                 && token.Equals($"Bearer {_emailSettings.DeliveryStatusToken}")))
            {
                _logger.LogError("Invalid Authorization Token in {method}", nameof(EmailDeliveryStatusCallback));
                return new UnauthorizedObjectResult("Missing or malformed 'Authorization' header.");
            }

            var result = await _emailDeliveryStatusService.HandleEmailDeliveryStatus(deliveryReceipt);

            return new OkObjectResult($"{result} record(s) updated.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating email status.");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
