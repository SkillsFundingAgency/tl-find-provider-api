using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Web.Samples.Pages;
public class ApiPageModel : PageModel
{
    public string ApiAppId { get; }
    public string ApiKey { get; }
    public string ApiUrl { get; }
    public string EoiApiUrl { get; }

    public ApiPageModel(
        IConfiguration config)
    {
        ApiAppId = config["FapApiSettings:AppId"];
        EoiApiUrl = config["FapApiSettings:EoiApiUri"];
        ApiKey = config["FapApiSettings:ApiKey"];
        ApiUrl = config["FapApiSettings:ApiUri"];
    }
}
