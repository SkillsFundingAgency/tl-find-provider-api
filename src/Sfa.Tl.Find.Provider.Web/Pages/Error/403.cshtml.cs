﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using System.Security.Claims;

namespace Sfa.Tl.Find.Provider.Web.Pages.Error;

public class Error403Model : PageModel
{
    private readonly ILogger<Error403Model> _logger;

    public Error403Model(ILogger<Error403Model> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (feature is null) return;

        if (User.Identity is { IsAuthenticated: true })
        {
            _logger.LogError(feature.Error, "Unexpected error occurred during request to path: {path} by user: {user}", feature.Path, User.FindFirstValue(CustomClaimTypes.UserId));
        }
        else
        {
            _logger.LogError(feature.Error, "Unexpected error occurred during request to {path}", feature.Path);
        }
    }
}