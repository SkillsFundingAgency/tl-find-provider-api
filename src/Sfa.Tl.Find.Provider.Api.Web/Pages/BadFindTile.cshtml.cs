using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Api.Web.Pages;

public class BadFindTileModel : PageModel
{
    private readonly ILogger<BadFindTileModel> _logger;
        
    public BadFindTileModel(IConfiguration config, ILogger<BadFindTileModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Page model {name} GET called",
                nameof(BadFindTileModel));
        }
    }
}