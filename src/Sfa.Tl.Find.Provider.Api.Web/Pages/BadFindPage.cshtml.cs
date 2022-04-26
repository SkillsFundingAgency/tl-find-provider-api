using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Api.Web.Pages
{
    public class BadFindPageModel : PageModel
    {
        private readonly ILogger<BadFindPageModel> _logger;
        
        public BadFindPageModel(IConfiguration config, ILogger<BadFindPageModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Page model {name} GET called",
                    nameof(BadFindPageModel));
            }
        }
    }
}
