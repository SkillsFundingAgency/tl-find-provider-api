﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ILogger<IndexModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            const string authenticatedUserStartUrl = "/dashboard";

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("User is already signed in at {currentPage}. Redirecting to  {redirectUrl}.",
                    nameof(IndexModel),
                    authenticatedUserStartUrl);
            }

            return RedirectToPage(authenticatedUserStartUrl);
        }

        return Page();
    }
}
