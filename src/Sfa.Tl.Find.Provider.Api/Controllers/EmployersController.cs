﻿using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Attributes;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Controllers;

[ApiController]
[ApiVersion("3.0")]
[HmacAuthorization]
[Route("api/v{version:apiVersion}/[controller]")]
[ResponseCache(NoStore = true, Duration = 0, Location = ResponseCacheLocation.None)]
public class EmployersController : ControllerBase
{
    private readonly IEmployerInterestService _employerInterestService;
    private readonly ILogger<EmployersController> _logger;

    public EmployersController(
        IEmployerInterestService employerInterestService,
        ILogger<EmployersController> logger)
    {
        _employerInterestService = employerInterestService ?? throw new ArgumentNullException(nameof(employerInterestService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a list of all routes.
    /// </summary>
    /// <returns>Json with routes.</returns>
    [HttpDelete, HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // ReSharper disable once StringLiteralTypo
    [Route("deleteinterest/{*id:guid}")]
    public async Task<IActionResult> DeleteInterest(Guid id)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug($"{nameof(EmployersController)} {nameof(DeleteInterest)} called.");
        }

        var deleted = await _employerInterestService.DeleteEmployerInterest(id);
        return deleted > 0
            ? NoContent()
            : NotFound();
    }
}