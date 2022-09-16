using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Samples.Pages;
public class IndexModel : PageModel
{
    public string ApiAppId { get; }
    public string ApiKey { get; }
    public string ApiUrl { get; }

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IConfiguration config, 
        ILogger<IndexModel> logger)
    {
        ApiAppId = config["FapApiSettings:AppId"];
        ApiKey = config["FapApiSettings:ApiKey"];
        ApiUrl = config["FapApiSettings:ApiUri"];

        _logger = logger;
    }

    public void OnGet()
    {

    }
}
