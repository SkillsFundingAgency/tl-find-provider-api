using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;
using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Web.Pages;

public class DashboardModel : PageModel
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<DashboardModel> _logger;

    public string? DisplayName { get; private set; }
    public string? Service { get; private set; }
    public string? UkPrn { get; private set; }

    public DashboardModel(
        IOptions<EmailSettings>? emailOptions,
        IEmailService emailService,
        ILogger<DashboardModel> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    public async Task OnGet()
    {
        var isAuthenticated = User.Identity.IsAuthenticated;

        var claims = User.Claims.ToList();
        foreach (var claim in claims)
        {
            Debug.WriteLine($"User claim {claim.Type} = {claim.Value}");
        }

        UkPrn = HttpContext.User.GetClaim(CustomClaimTypes.UkPrn);
        DisplayName = HttpContext.User.GetClaim(CustomClaimTypes.DisplayName);
        Service = HttpContext.User.GetClaim(CustomClaimTypes.Service);
    }

    public async Task OnPost()
    {
        const string template = "TestWithoutPersonalisation";
        var tokens = new Dictionary<string, string>();
        var recipients = _emailSettings.SupportEmailAddress;

        var sent = await _emailService.SendEmail(recipients, template, tokens);

        if (sent)
            _logger.LogInformation("Sent one email");
        else
            _logger.LogWarning("Failed to send email");
    }
}