using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Samples.Pages;

public class UnsubscribeModel : PageModel
{
    public string ApiAppId { get; }
    public string ApiKey { get; }
    public string ApiUrl { get; }

    public UnsubscribeModel(
        IConfiguration config)
    {
        ApiAppId = config["FapApiSettings:AppId"];
        ApiKey = config["FapApiSettings:ApiKey"];
        ApiUrl = config["FapApiSettings:ApiUri"];
    }

    public void OnGet(Guid id)
    {
    }
}