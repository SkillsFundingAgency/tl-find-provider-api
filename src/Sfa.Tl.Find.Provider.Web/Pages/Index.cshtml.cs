using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Pages;
public class IndexModel : PageModel
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<IndexModel> _logger;

    public string? DisplayName { get; private set; }
    public string? Service { get; private set; }
    public string? UkPrn { get; private set;  }

    public IndexModel(
        IOptions<EmailSettings>? emailOptions,
        IEmailService emailService,
        ILogger<IndexModel> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    public async Task OnGet()
    {
        UkPrn = HttpContext.User.GetClaim(ProviderClaims.ProviderUkprn);
        DisplayName = HttpContext.User.GetClaim(ProviderClaims.DisplayName);
        Service = HttpContext.User.GetClaim(ProviderClaims.Service);
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
