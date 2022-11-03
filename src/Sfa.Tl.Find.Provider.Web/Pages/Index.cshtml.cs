using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Web.Pages;
public class IndexModel : PageModel
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly IQualificationRepository _qualificationRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IOptions<EmailSettings>? emailOptions,
        IEmailService emailService,
        IQualificationRepository qualificationRepository,
        ILogger<IndexModel> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
        _emailSettings = emailOptions?.Value
                         ?? throw new ArgumentNullException(nameof(emailOptions));
    }

    public async Task OnGet()
    {
        var qualifications = await _qualificationRepository.GetAll();

        foreach (var qualification in qualifications)
        {
            Debug.WriteLine($"{qualification.Id} - {qualification.Name}");
        }
    }

    public async Task OnPost()
    {
        var template = "TestWithoutPersonalisation";

        var tokens = new Dictionary<string, string>();

        var recipients = _emailSettings.SupportEmailAddress;
        var sent = await _emailService.SendEmail(recipients, template, tokens);

        if (sent)
            _logger.LogInformation("Sent one email");
        else
            _logger.LogWarning("Failed to send email");

    }
}
