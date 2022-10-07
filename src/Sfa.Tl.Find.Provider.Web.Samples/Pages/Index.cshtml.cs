namespace Sfa.Tl.Find.Provider.Web.Samples.Pages;
public class IndexModel : ApiPageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IConfiguration config,
        ILogger<IndexModel> logger) : base(config)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
