﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Pages.Error;

public class Error404Model : PageModel
{
    private readonly ILogger<Error404Model> _logger;

    public Error404Model(ILogger<Error404Model> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
    }
}