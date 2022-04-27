using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Api.Web.Pages;

public class FindTileModel : PageModel
{
    private readonly ILogger<FindTileModel> _logger;
        
    public string SearchApiUrl { get; }

    public FindTileModel(IConfiguration config, ILogger<FindTileModel> logger)
    {
        _logger = logger;

        SearchApiUrl = config["SearchApiUrl"];
    }

    public void OnGet()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Page model {name} GET called",
                nameof(FindTileModel));
        }
    }
}