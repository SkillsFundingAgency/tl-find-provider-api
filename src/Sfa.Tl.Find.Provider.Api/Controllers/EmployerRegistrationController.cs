using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmployerRegistrationController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmployerRegistrationController> _logger;

    public EmployerRegistrationController(
        IEmailService emailService,
        ILogger<EmployerRegistrationController> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Register employer interest.
    /// </summary>
    /// <param name="employerRegistration">Employer registration detals</param>
    /// <returns>A list of results.</returns>
    [HttpPost]
    [Route("register", Name = "RegisterEmployerInterest")]
    [ProducesResponseType(typeof(IEnumerable<Town>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterInterest(EmployerRegistration employerRegistration)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(EmployerRegistrationController)} {nameof(RegisterInterest)} called.");
        }

        //TODO: Validation

        var providerList = GenerateProviderList(employerRegistration.Providers);
        var sent = await _emailService.SendEmployerInterestEmail(
            employerRegistration.EmployerName,
            employerRegistration.EmployerTelephone,
            employerRegistration.EmployerEmail,
            providerList);
        return Ok(sent);
    }

    private string GenerateProviderList(IEnumerable<Models.Provider> providers)
    {
        if(providers is null) return null;

        var sb = new StringBuilder();

        foreach (var provider in providers.OrderBy(p => p.Name))
        {
            sb.AppendLine($"# {provider.Name}");
            sb.AppendLine($"* Location: {provider.Town} {provider.Postcode}");
            sb.AppendLine($"* {FormatContactDetails(provider.Telephone, provider.Email, provider.Website)}");
            
            sb.AppendLine("");
        }

        return sb.ToString();
    }
    private static string FormatContactDetails(string phone, string email, string website)
    {
        var hasPhone = !string.IsNullOrWhiteSpace(phone);
        var hasEmail = !string.IsNullOrWhiteSpace(email);
        var hasWebsite = !string.IsNullOrWhiteSpace(website);

        var sb = new StringBuilder();

        if (hasPhone || hasEmail || hasWebsite)
        {
            if (hasPhone)
            {
                sb.Append($"Telephone: {phone}");
                if (hasEmail)
                {
                    sb.Append("; ");
                }
            }

            if (hasEmail)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }
                sb.Append($"Email: {email}");
            }

            if (hasWebsite)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }

                sb.Append($"Website: {website}");
            }
        }

        return sb.ToString();
    }

}